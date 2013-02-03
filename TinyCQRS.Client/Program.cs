using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TinyCQRS.Domain;
using TinyCQRS.Domain.EventSourced.QualityAssurance;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Infrastructure.Caching;
using TinyCQRS.Infrastructure.Persistence;
using TinyCQRS.Messages;
using TinyCQRS.ReadModel.Generators;
using TinyCQRS.ReadModel.Infrastructure;
using TinyCQRS.ReadModel.Model;

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

			var service = DatabaseBacked();
			
			CreateNewSite(service, siteId);

			//LoadExistingData(service, existing);
			CrawlSite(service, siteId);

			if (_readModelContext != null)
			{
				Console.WriteLine("Completing outstanding readmodel saves");
				var c = _readModelContext.ExecuteDelayedCommits();
				Console.WriteLine("Completed {0} commits", c);
			}

			Console.WriteLine("rbeak");
		}

		static void CreateNewSite(ISiteCrawlService service, Guid siteId)
		{
			service.CreateNewSite(siteId, "Testsite", "http://root.dk");

			CrawlSite(service, siteId);
		}

		static void CrawlSite(ISiteCrawlService service, Guid siteId)
		{
			var crawlId = Guid.NewGuid();
			var crawler = new Crawler(service, new NullLogger());

			crawler.Crawl(siteId, crawlId);

			for (var i = 0; i < 8000; i++)
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

		static ISiteCrawlService DatabaseBacked()
		{
			// Infrastructure
			var messageBus = new InMemoryMessageBus();
			_eventStore = new SqlEventStore(@"Data Source=.\SQLExpress;Integrated Security=true;Database=TinyCQRS.Events");
			var eventStore = new CachingEventStore(_eventStore, new MemoryCache<IList<Event>>());
			var dispatchingEventStore = new DispatchingEventStore(eventStore, messageBus);

			// Write-side
			var aggregateCache = new MemoryCache<AggregateRoot>();
			var siteRepository = new EventedRepository<SiteAggregate>(dispatchingEventStore, aggregateCache);
			var crawlRepository = new EventedRepository<CrawlAggregate>(dispatchingEventStore, aggregateCache);
			var siteCommandHandler = new SiteCommandHandler(siteRepository);
			var crawlCommandHandler = new CrawlCommandHandler(crawlRepository);

			// Read-side
			_readModelContext = new ReadModelContext(true);
			var siteDtoRepository = new CachingReadModelRepository<Site>(new EfReadModelRepository<Site>(_readModelContext));
			var pageDtoRepository = new CachingReadModelRepository<Page>(new EfReadModelRepository<Page>(_readModelContext));
			var crawlDtoRepository = new CachingReadModelRepository<CrawlJob>(new EfReadModelRepository<CrawlJob>(_readModelContext)); ;
			var siteReadModelGenerator = new SiteReadModelGenerator(siteDtoRepository, pageDtoRepository);
			var pageReadModelGenerator = new PageReadModelGenerator(pageDtoRepository);
			var crawlReadModelGenerator = new CrawlRunReadModelGenerator(crawlDtoRepository, siteDtoRepository);

			// Hook-up
			messageBus.Subscribe(pageReadModelGenerator, siteReadModelGenerator, crawlReadModelGenerator);

			// Service
			return new SiteCrawlService(siteCommandHandler, crawlCommandHandler, siteDtoRepository, crawlDtoRepository);
		}

		static ISiteCrawlService MemoryBacked()
		{
			// Infrastructure
			var messageBus = new InMemoryMessageBus();
			var eventStore = new InMemoryEventStore();
			//var eventStore = new SqlEventStore(@"Data Source=.\SQLExpress;Integrated Security=true;Database=TinyCQRS.Events");
			_eventStore = new CachingEventStore(eventStore, new MemoryCache<IList<Event>>());
			var dispatchingEventStore = new DispatchingEventStore(_eventStore, messageBus);

			// Write-side
			var aggregateCache = new MemoryCache<AggregateRoot>();
			var siteRepository = new EventedRepository<SiteAggregate>(dispatchingEventStore, aggregateCache);
			var crawlRepository = new EventedRepository<CrawlAggregate>(dispatchingEventStore, aggregateCache);
			var siteCommandHandler = new SiteCommandHandler(siteRepository);
			var crawlCommandHandler = new CrawlCommandHandler(crawlRepository);

			// Read-side
			var siteDtoRepository = new InMemoryReadModelRepository<Site>();
			var pageDtoRepository = new InMemoryReadModelRepository<Page>();
			var crawlDtoRepository = new InMemoryReadModelRepository<CrawlJob>();
			var siteReadModelGenerator = new SiteReadModelGenerator(siteDtoRepository, pageDtoRepository);
			var pageReadModelGenerator = new PageReadModelGenerator(pageDtoRepository);
			var crawlReadModelGenerator = new CrawlRunReadModelGenerator(crawlDtoRepository, siteDtoRepository);

			// Hook-up
			messageBus.Subscribe(pageReadModelGenerator, siteReadModelGenerator, crawlReadModelGenerator);

			// Service
			return new SiteCrawlService(siteCommandHandler, crawlCommandHandler, siteDtoRepository, crawlDtoRepository);
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
