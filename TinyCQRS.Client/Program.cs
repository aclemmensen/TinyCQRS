using System;
using System.Threading;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using TinyCQRS.Application.Crosscutting;
using TinyCQRS.Application.Modules.Crawler;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Services;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Infrastructure;
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

			var container = Container(new DatabaseServiceInstaller(
				@"Data Source=.\SQLExpress;Integrated Security=true;Database=TinyCQRS.Events",
				@"Data Source=.\SQLExpress;Integrated Security=true;Database=TinyCQRS.ReadModelDenormalized",
				@"mongodb://localhost"));
			container.Register(Component.For<ILogger>().ImplementedBy<ConsoleLogger>().IsDefault());

			_eventStore = container.Resolve<IEventStore>();

			var crawlService = container.Resolve<ISiteCrawlService>();
			var siteService = container.Resolve<ISiteService>();
			var setupService = container.Resolve<ISetupService>();
			
			setupService.CreateNewSite(new CreateNewSite(siteId, "Testsite", "http://weeee.dk"));
			siteService.OrderFullCrawl(new OrderCrawl(Guid.NewGuid(), siteId, DateTime.UtcNow));

			//crawler.Crawl(siteId);

			//for (var i = 0; i < 250; i++)
			//{
			//	crawler.Handle("http://someurl.dk/page_" + i + ".html", "This is the content");
			//	crawler.Handle("http://newurl.dk", "this is a new page");
			//}

			if (_readModelContext != null)
			{
				Console.WriteLine("Completing outstanding readmodel saves");
				var c = _readModelContext.ExecuteDelayedCommits();
				Console.WriteLine("Completed {0} commits", c);
			}

			Console.WriteLine("rbeak");
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

			SiteCrawlServiceTest();
		}
    }
}
