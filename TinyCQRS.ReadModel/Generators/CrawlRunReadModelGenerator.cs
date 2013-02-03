using System;
using TinyCQRS.Messages;
using TinyCQRS.Messages.Events;
using TinyCQRS.ReadModel.Interfaces;
using TinyCQRS.ReadModel.Model;

namespace TinyCQRS.ReadModel.Generators
{
	public class CrawlRunReadModelGenerator :
		IConsume<CrawlStarted>,
		IConsume<PageChecked>
	{
		private readonly IReadModelRepository<CrawlJob> _crawls;
		private readonly IReadModelRepository<Site> _sites;

		public CrawlRunReadModelGenerator(IReadModelRepository<CrawlJob> crawls, IReadModelRepository<Site> sites)
		{
			_crawls = crawls;
			_sites = sites;
		}

		public void Process(PageChecked @event)
		{
			var crawl = _crawls.Get(@event.AggregateId);

			crawl.Records.Add(new CrawlRecord
			{
				Id = Guid.NewGuid(),
				CrawlJob = crawl,
				CrawlJobId = crawl.Id,
				PageId = @event.PageId,
				TimeOfCheck = @event.TimeOfCheck
			});

			_crawls.Commit();
		}

		public void Process(CrawlStarted @event)
		{
			var site = _sites.Get(@event.SiteId);

			var crawl = _crawls.Create();
			crawl.Id = @event.AggregateId;
			crawl.Site = site;
			crawl.SiteId = site.Id;
			crawl.StartTime = @event.StartTime;

			_crawls.Add(crawl);

			_crawls.Commit();
		}
	}
}