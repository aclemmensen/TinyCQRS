using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Infrastructure.Caching;
using TinyCQRS.Infrastructure.Persistence;
using TinyCQRS.Messages;
using TinyCQRS.Messages.Commands;
using TinyCQRS.ReadModel.Generators;
using TinyCQRS.ReadModel.Infrastructure;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.Client
{
	class Program
	{
		private static ReadModelContext _readModelContext;
		private static IEventStore _eventStore;

		static void SiteCrawlServiceTest()
		{
			var siteId = Guid.NewGuid();
			var crawlId = Guid.NewGuid();

			//var service = DatabaseBacked();

			var container = Container(new DatabaseServiceInstaller(@"Data Source=.\SQLExpress;Integrated Security=true;Database=TinyCQRS.Events"));
			//var container = Container(new MemoryServiceInstaller());
			var service = container.Resolve<ISiteCrawlService>();
			_eventStore = container.Resolve<IEventStore>();

			CreateNewSite(service, siteId);

			//LoadExistingData(service, existing);
			CrawlSite(service, siteId);

			LoadExistingData(service, siteId);

			if (_readModelContext != null)
			{
				Console.WriteLine("Completing outstanding readmodel saves");
				var c = _readModelContext.ExecuteDelayedCommits();
				Console.WriteLine("Completed {0} commits", c);
			}

			var site = service.GetSite(siteId);
			Console.WriteLine(site);

			Console.WriteLine("rbeak");
		}

		static void CreateNewSite(ISiteCrawlService service, Guid siteId)
		{
			service.CreateNewSite(new CreateNewSite(siteId, "Testsite", "http://weeee.dk"));
		}

		static void CrawlSite(ISiteCrawlService service, Guid siteId)
		{
			var crawler = new Crawler(service, new NullLogger());

			crawler.Crawl(siteId);

			for (var i = 0; i < 250; i++)
			{
				crawler.Handle("http://someurl.dk/page_" + i + ".html", "This is the content");
				crawler.Handle("http://newurl.dk", "this is a new page");
			}
		}

		static void LoadExistingData(ISiteCrawlService service, Guid siteId)
		{
			var site = service.GetSite(siteId);

			Console.WriteLine("rbeak");
		}


		//static ISiteCrawlService MemoryBacked()
		//{
		//	// Infrastructure
			
		//	var messageBus = new InMemoryMessageBus();
		//	_eventStore = new InMemoryEventStore();
		//	var eventStore = new CachingEventStore(_eventStore, new MemoryCache<IList<Event>>());
		//	var dispatchingEventStore = new DispatchingEventStore(eventStore, messageBus);
		//	var logger = new NullLogger();

		//	// Write-side
		//	var aggregateCache = new MemoryCache<AggregateRoot>();
		//	var siteRepository = new EventedRepository<SiteAggregate>(dispatchingEventStore, aggregateCache);
		//	var crawlRepository = new EventedRepository<CrawlAggregate>(dispatchingEventStore, aggregateCache);
		//	var container = Container(siteRepository, crawlRepository);
		//	var dispatcher = new CommandDispatcher(container, logger);

		//	// Read-side
		//	var siteDtoRepository = new InMemoryReadModelRepository<Site>();
		//	var pageDtoRepository = new InMemoryReadModelRepository<Page>();
		//	var siteReadModelGenerator = new SiteReadModelGenerator(siteDtoRepository);
		//	var pageReadModelGenerator = new PageReadModelGenerator(pageDtoRepository);

		//	// Hook-up
		//	messageBus.Subscribe(pageReadModelGenerator, siteReadModelGenerator);

		//	// Service
		//	return new SiteCrawlService(dispatcher, siteDtoRepository);
		//}

		static IWindsorContainer Container(IWindsorInstaller installer)
		{
			var container = new WindsorContainer();
			container.Install(installer);

			var consumers = container.ResolveAll<IConsume>();
			var bus = container.Resolve<IMessageBus>();
			bus.Subscribe(consumers);

			return container;
		}

		public class ServiceInstaller : IWindsorInstaller
		{
			public virtual void Install(IWindsorContainer container, IConfigurationStore store)
			{
				container.Register(Classes.FromAssemblyContaining(typeof (IHandle<>)).BasedOn<IHandler>().WithService.AllInterfaces());
				container.Register(Classes.FromAssemblyContaining<SiteReadModelGenerator>().BasedOn<IConsume>().WithServiceAllInterfaces());
				
				container.Register(Component.For<IMessageBus>().ImplementedBy<InMemoryMessageBus>());
				container.Register(Component.For(typeof (ICache<>)).ImplementedBy(typeof (MemoryCache<>)));
				container.Register(Component.For(typeof (IRepository<>)).ImplementedBy(typeof (EventedRepository<>)));
				container.Register(Component.For<ICommandDispatcher>().ImplementedBy<CommandDispatcher>());
				container.Register(Component.For<ILogger>().ImplementedBy<NullLogger>());
				container.Register(Component.For<IWindsorContainer>().Instance(container));

				container.Register(
					Component.For<IEventStore>().ImplementedBy<DispatchingEventStore>(),
					Component.For<IEventStore>().ImplementedBy<CachingEventStore>());

				container.Register(Component.For<ISiteCrawlService>().ImplementedBy<SiteCrawlService>());
			}
		}

		public class DatabaseServiceInstaller : ServiceInstaller
		{
			private readonly string _connstr;

			public DatabaseServiceInstaller(string connstr)
			{
				_connstr = connstr;
			}

			public override void Install(IWindsorContainer container, IConfigurationStore store)
			{
				base.Install(container, store);

				container.Register(Component.For<IEventStore>().ImplementedBy<SqlEventStore>().DependsOn(new {connstr = _connstr}));
				container.Register(Component.For<DbContext>().ImplementedBy<ReadModelContext>());

				container.Register(
					Component.For(typeof (IReadModelRepository<>)).ImplementedBy(typeof (CachingReadModelRepository<>)),
					Component.For(typeof (IReadModelRepository<>)).ImplementedBy(typeof (EfReadModelRepository<>)));
			}
		}

		public class MemoryServiceInstaller : ServiceInstaller
		{
			public override void Install(IWindsorContainer container, IConfigurationStore store)
			{
				base.Install(container, store);

				container.Register(Component.For<IEventStore>().ImplementedBy<InMemoryEventStore>());
				container.Register(Component.For(typeof (IReadModelRepository<>)).ImplementedBy(typeof (InMemoryReadModelRepository<>)));
			}
		}

		static void Main(string[] args)
		{
			ThreadPool.SetMinThreads(5, 5);

			Task.Run(async () =>
			{
				var lastProcessed = 0;

				while (true)
				{
					if (_eventStore != null)
					{
						long diffProcessed = _eventStore.Processed - lastProcessed;
						lastProcessed = _eventStore.Processed;

						if (diffProcessed != 0)
						{
							Console.WriteLine("Processed {0} events ({1} events/sec)", _eventStore.Processed, diffProcessed);
						}
					}

					await Task.Delay(1000);
				}
			});

			SiteCrawlServiceTest();
		}
    }
}
