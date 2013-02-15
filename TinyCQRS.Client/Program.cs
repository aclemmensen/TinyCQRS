using System;
using System.Threading;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using TinyCQRS.Application.Crosscutting;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Services;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Domain.Models.QualityAssurance;
using TinyCQRS.Infrastructure;
using TinyCQRS.Infrastructure.Persistence;
using TinyCQRS.ReadModel.Infrastructure;

namespace TinyCQRS.Client
{
	class Program
	{
		private static ReadModelContext _readModelContext;
		private static IEventStore _eventStore;

		static void SiteCrawlServiceTest()
		{
			//var siteId = new Guid("d7af265a-1ffd-456c-9be4-8ae74f3b60a8");
			var siteId = Guid.NewGuid();

			var container = Container(new DatabaseServiceInstaller(@"Data Source=.\SQLExpress;Integrated Security=true;Database=TinyCQRS.Events", @"Data Source=.\SQLExpress;Integrated Security=true;Database=TinyCQRS.ReadModelDenormalized", @"mongodb://localhost"));
			//var container = Container(new MemoryServiceInstaller());
			container.Register(Component.For<ILogger>().ImplementedBy<NullLogger>().IsDefault());

			_eventStore = container.Resolve<IEventStore>();

			var crawlService = container.Resolve<ICrawlService>();
			var siteService = container.Resolve<ISiteService>();
			var setupService = container.Resolve<ISetupService>();
			
			setupService.CreateNewSite(new CreateNewSite(siteId, "Testsite", "http://weeee.dk"));
			setupService.CreateSpellcheckConfiguration(new CreateSpellcheckConfiguration(siteId, "None", "None", new string[0], new string[0]));
			siteService.OrderFullCrawl(new OrderCrawl(Guid.NewGuid(), siteId, DateTime.UtcNow));

			if (_readModelContext != null)
			{
				Console.WriteLine("Completing outstanding readmodel saves");
				var c = _readModelContext.ExecuteDelayedCommits();
				Console.WriteLine("Completed {0} commits", c);
			}

			Console.WriteLine("rbeak");
		}

		static void PerformanceTest()
		{
			var container = Container(new DatabaseServiceInstaller());

			container.Register(Component.For<IBlobStorage>().ImplementedBy<MemoryBlobStorage>().IsDefault());

			_eventStore = container.Resolve<IEventStore>();
			var repository = container.Resolve<IRepository<Crawl>>();

			var siteId = Guid.NewGuid();

			var setupService = container.Resolve<ISetupService>();
			setupService.CreateNewSite(new CreateNewSite(siteId, "Perftest", "Perftest"));

			var msgbus = container.Resolve<IMessageBus>();
			msgbus.ClearSubscribers();

			var id = Guid.NewGuid();
			var pageid = Guid.NewGuid();

			var crawl = new Crawl(id, siteId, DateTime.UtcNow);
			crawl.StartCrawl("Perftest crawler", DateTime.UtcNow);
			var blobstorage = container.Resolve<IBlobStorage>();

			crawl.AddNewPage(pageid, "perftesturl", "nocontent", DateTime.UtcNow, blobstorage);

			repository.Save(crawl);

			var now = DateTime.UtcNow;

			for (var i = 0; i < 10000000; i++)
			{
				crawl.PageCheckedWithoutChange(pageid, now);
				repository.Save(crawl);
			}

			Console.WriteLine("break");
		}

		static IWindsorContainer Container(IWindsorInstaller installer)
		{
			var container = new WindsorContainer();
			container.Install(installer);

			return container;
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

			//SiteCrawlServiceTest();
			PerformanceTest();
		}
    }
}
