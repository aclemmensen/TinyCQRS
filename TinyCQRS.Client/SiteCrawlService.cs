using System;
using System.Collections.Generic;
using System.Linq;
using TinyCQRS.Domain.EventSourced.QualityAssurance;
using TinyCQRS.Messages.Commands;
using TinyCQRS.ReadModel.Interfaces;
using TinyCQRS.ReadModel.Model;

namespace TinyCQRS.Client
{
	public class SiteCrawlService : ISiteCrawlService
	{
		private readonly SiteCommandHandler _siteHandler;
		private readonly CrawlCommandHandler _crawlHandler;
		private readonly IReadModelRepository<Site> _siteRepository;
		private readonly IReadModelRepository<CrawlJob> _crawlRepository;

		public SiteCrawlService(
			SiteCommandHandler siteHandler, 
			CrawlCommandHandler crawlHandler, 
			IReadModelRepository<Site> siteRepository,
			IReadModelRepository<CrawlJob> crawlRepository)
		{
			_siteHandler = siteHandler;
			_crawlHandler = crawlHandler;

			_siteRepository = siteRepository;
			_crawlRepository = crawlRepository;
		}

		#region WRITE

		public void CreateNewSite(Guid id, string name, string root)
		{
			_siteHandler.Handle(new CreateNewSite(id, name, root));
		}

		public void StartCrawl(Guid crawlId, Guid siteId)
		{
			_crawlHandler.Handle(new StartCrawl(crawlId, siteId, DateTime.UtcNow));
		}

		public void AddNewPage(Guid crawlId, Guid pageId, string url, string content)
		{
			var crawl = _crawlRepository.Get(crawlId);

			_siteHandler.Handle(new CreateNewPage(crawl.SiteId, pageId, url, content));
			RegisterCheck(crawlId, pageId);
		}

		public void UpdatePageContent(Guid crawlId, Guid pageId, string newContent)
		{
			var crawl = _crawlRepository.Get(crawlId);
			_siteHandler.Handle(new UpdatePageContent(crawl.SiteId, pageId, newContent));
			RegisterCheck(crawlId, pageId);
		}

		public void PageCheckedWithoutChanges(Guid crawlId, Guid pageId, DateTime timeOfCheck)
		{
			RegisterCheck(crawlId, pageId);
		}

		private void RegisterCheck(Guid crawlId, Guid pageId)
		{
			_crawlHandler.Handle(new RegisterCheck(crawlId, pageId, DateTime.UtcNow));
		}

		#endregion

		#region READ

		public Site GetSite(Guid siteId)
		{
			return _siteRepository.Get(siteId);
		}

		public IEnumerable<Page> GetPagesFor(Guid siteId)
		{
			return _siteRepository.Get(siteId).Pages;
		}

		public CrawlSpec GetCrawlInfoFor(Guid crawlId)
		{
			var crawl = _crawlRepository.Get(crawlId);

			return new CrawlSpec(crawlId, crawl.Site);
		}

		#endregion
	}

	public class CrawlSpec
	{
		public Guid CrawlId { get; set; }

		public Guid SiteId { get; set; }
		public string Root { get; set; }

		private readonly List<PageInfo> _pageInfo;
		public IEnumerable<PageInfo> Pages { get { return _pageInfo; } }

		public CrawlSpec(Guid crawlId, Site site)
		{
			CrawlId = crawlId;
			SiteId = site.Id;
			Root = site.Root;

			_pageInfo = site.Pages.Select(page => new PageInfo
			{
				PageId = page.Id,
				Url = page.Url,
				ContentHash = HashingHelper.Hash(page.Content)
			}).ToList();
		}
	}

	public class PageInfo
	{
		public Guid PageId { get; set; }
		public string Url { get; set; }
		public string ContentHash { get; set; }
	}
}