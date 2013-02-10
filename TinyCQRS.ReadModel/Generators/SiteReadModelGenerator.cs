using System;
using System.Linq;
using TinyCQRS.Contracts;
using TinyCQRS.Contracts.Events;
using TinyCQRS.Contracts.Models;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Generators
{
	public class SiteReadModelGenerator : 
		IConsume<SiteCreatedEvent>,
		IConsume<PageCreated>
	{
		private readonly IReadModelRepository<Site> _sites;

		public SiteReadModelGenerator(IReadModelRepository<Site> sites)
		{
			_sites = sites;
		}

		public void Process(SiteCreatedEvent @event)
		{
			var site = _sites.Create();
			
			site.GlobalId = @event.AggregateId;
			site.Name = @event.Name;
			site.Root = @event.Root;

			_sites.Add(site);
			_sites.Commit();
		}

		public void Process(PageCreated @event)
		{
			var site = _sites.Get(@event.SiteId);

			var page = new Page
			{
				GlobalId = @event.PageId,
				Content = @event.Content,
				FirstSeen = @event.TimeOfCreation,
				Url = @event.Url,
				SiteId = @event.SiteId
			};

			//page.Checks.Add(new PageCheck
			//{
			//	Page = page,
			//	PageId = @event.PageId,
			//	TimeOfCheck = @event.TimeOfCreation,
			//	CrawlId = @event.AggregateId
			//});

			site.Pages.Add(page);

			_sites.Update(site);
			_sites.Commit();
			
		}
	}

	public class SiteIdentityReadModelGenerator :
		IConsume<SiteCreatedEvent>,
		IConsume<CrawlCompleted>
	{
		private readonly IReadModelRepository<SiteIdentity> _read;
		private readonly IReadModelRepository<Crawl> _crawls;

		public SiteIdentityReadModelGenerator(IReadModelRepository<SiteIdentity> read, IReadModelRepository<Crawl> crawls)
		{
			_read = read;
			_crawls = crawls;
		}

		public void Process(SiteCreatedEvent @event)
		{
			var site = _read.Create();

			site.GlobalId = @event.AggregateId;
			site.Name = @event.Name;
			site.Root = @event.Root;
			site.PageCount = 0;

			_read.Add(site);
			_read.Commit();
		}

		public void Process(CrawlCompleted @event)
		{
			var crawl = _crawls.Get(@event.AggregateId);
			var site = _read.Get(crawl.SiteId);

			var total = @event.NewPages.Count() + @event.UnchangedPages.Count() + @event.ChangedPages.Count();

			site.PageCount = total;

			_read.Update(site);
			_read.Commit();
		}
	}

	public class SiteOverviewReadModelGenerator :
		IConsume<CrawlOrdered>,
		IConsume<CrawlCompleted>
	{
		private readonly IReadModelRepository<SiteOverview> _read;
		private readonly IReadModelRepository<Crawl> _crawls;

		public SiteOverviewReadModelGenerator(IReadModelRepository<SiteOverview> read, IReadModelRepository<Crawl> crawls)
		{
			_read = read;
			_crawls = crawls;
		}

		public void Process(CrawlCompleted @event)
		{
			var crawl = _crawls.Get(@event.AggregateId);
			var r = new Random();
			
			_read.CreateOrUpdate(crawl.SiteId, site =>
			{
				var brokenLinks = r.Next(0, 15);
				var misspellings = r.Next(0, 15);

				site.BrokenLinksCount = new ErrorCount(r.Next(brokenLinks, 25), brokenLinks, r.Next(5));
				site.MisspellingsCount = new ErrorCount(r.Next(misspellings, 25), misspellings, r.Next(5));

				site.History.Add(new CrawlStatusItem
				{
					BrokenLinks = brokenLinks,
					Misspellings = misspellings,
					Time = @event.TimeOfCompletion
				});

				site.LastCrawl = @event.TimeOfCompletion;
			});
		}

		public void Process(CrawlOrdered @event)
		{
			_read.CreateOrUpdate(@event.SiteId, site =>
			{
				site.NextCrawl = @event.TimeOfOrder;
			});
		}
	}
}