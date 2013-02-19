using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using TinyCQRS.Application.Crosscutting;
using TinyCQRS.Contracts;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Services;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Domain.Models.QualityAssurance;
using TinyCQRS.Infrastructure;
using TinyCQRS.Infrastructure.Caching;
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

			var container = Container(new DatabaseServiceInstaller());
			//var container = Container(new MemoryServiceInstaller());
			//container.Register(Component.For<ILogger>().ImplementedBy<NullLogger>().IsDefault());

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

			Console.WriteLine("Hit [enter] to quit");
			Console.ReadLine();
		}

		static void PerformanceTest()
		{
			var stores = new Type[]
			{
				//typeof (InMemoryEventStore),
				//typeof (OrmLiteEventStore),
				//typeof (MongoEventStore),
				typeof (RedisEventStore),
			};

			ThreadPool.SetMinThreads(16, 16);
			ThreadPool.SetMaxThreads(32, 32);

			Timing();
			const bool cache = true;

			foreach (var type in stores)
			{
				PerformanceTestWith(type, 1, cache);

				for (var i = 2; i <= 10; i+=2)
				{
					PerformanceTestWith(type, i, cache);
				}
			}
		}

		static void PerformanceTestWith(Type eventStoreType, int parallel, bool useCaching = false)
		{
			Console.WriteLine("PERFTEST: {0}, {1} client(s)", eventStoreType.Name, parallel);
			var container = Container(new DatabaseServiceInstaller());
			
			container.Register(Component.For<IBlobStorage>().ImplementedBy<NullBlobStorage>().IsDefault());
			container.Register(Component.For<ILogger>().ImplementedBy<NullLogger>().IsDefault());
			if(useCaching)
				container.Register(Component.For<IEventStore>().ImplementedBy<CachingEventStore>());
			
			container.Register(Component.For<IEventStore>().ImplementedBy(eventStoreType).Named("es"));
			
			_eventStore = container.Resolve<IEventStore>();
			var repository = container.Resolve<IRepository<Crawl>>();
			var setupService = container.Resolve<ISetupService>();
			var crawlService = container.Resolve<ICrawlService>();
			var msgbus = container.Resolve<IMessageBus>();
			var blobstorage = container.Resolve<IBlobStorage>();

			msgbus.ClearSubscribers();

			var tasks = new List<Task>();
			var random = new Random();

			var shouldCancel = false;

			Task.Run(async () =>
			{
				await Task.Delay(10000);
				shouldCancel = true;
			});

			for (var y = 1; y <= parallel; y++)
			{
				var x = y;
				var t = Task.Run(() =>
				{
					//await Task.Delay(random.Next(0, 750));
					//Console.WriteLine("Staring task {0}", x);
					
					var siteId = Guid.NewGuid();
					var crawlId = Guid.NewGuid();
					var pageid = Guid.NewGuid();

					setupService.CreateNewSite(new CreateNewSite(siteId, "Perftest", "Perftest"));

					var crawl = new Crawl(crawlId, siteId, DateTime.UtcNow);
					crawl.StartCrawl("Perftest crawler", DateTime.UtcNow);

					crawl.AddNewPage(pageid, "perftesturl", "nocontent", DateTime.UtcNow, blobstorage);

					repository.Save(crawl);

					for (var i = 0; i < 50000000000; i++)
					{
						//crawl.PageCheckedWithoutChange(pageid, DateTime.UtcNow);
						crawlService.PageCheckedWithoutChanges(new RegisterCheckWithoutChange(crawlId, pageid, DateTime.UtcNow));

						if (i%100 == 0 && shouldCancel)
						{
							break;
						}
					}
				});

				tasks.Add(t);
			}

			Task.WaitAll(tasks.ToArray());
			container.Dispose();

			Console.WriteLine("... done\n");
		}

		static IWindsorContainer Container(IWindsorInstaller installer)
		{
			var container = new WindsorContainer();
			container.Install(installer);

			return container;
		}

		static void Main(string[] args)
		{
			//SiteCrawlServiceTest();
			PerformanceTest();
		}


		private static bool _isTiming;

		static void Timing()
		{
			if (_isTiming) return;

			Task.Run(async () =>
			{
				var lastProcessed = 0;
				var lastTime = DateTime.Now;

				while (true)
				{
					if (_eventStore != null)
					{
						var now = DateTime.Now;
						long diffProcessed = _eventStore.Processed - lastProcessed;
						lastProcessed = _eventStore.Processed;

						var persec = diffProcessed != 0 ? diffProcessed / (now - lastTime).TotalSeconds : 0;

						if(persec >= 0)
							Console.WriteLine("Processed {0} events ({1:##} events/sec)", _eventStore.Processed, persec);

						lastTime = now;
					}

					await Task.Delay(1000);
				}
			});

			_isTiming = true;
		}
    }
}
