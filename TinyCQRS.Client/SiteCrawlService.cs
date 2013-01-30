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
		private readonly PageCommandHandler _pageHandler;
		private readonly IDtoRepository<SiteDto> _siteRepository;

		public SiteCrawlService(SiteCommandHandler siteHandler, PageCommandHandler pageHandler, IDtoRepository<SiteDto> siteRepository)
		{
			_siteHandler = siteHandler;
			_pageHandler = pageHandler;

			_siteRepository = siteRepository;
		}

		#region WRITE

		public void CreateNewSite(Guid id, string name, string root)
		{
			_siteHandler.Handle(new CreateNewSite(id, name, root));
		}

		public void AddNewPage(Guid siteId, Guid pageId, string url, string content)
		{
			_pageHandler.Handle(new CreatePage(pageId, url, content));
			_siteHandler.Handle(new AddPageToSite(siteId, pageId));
		}

		public void UpdatePageContent(Guid siteId, Guid pageId, string newContent)
		{
			_pageHandler.Handle(new UpdatePageContent(pageId, newContent));
		}

		#endregion

		#region READ

		public SiteDto GetSite(Guid siteId)
		{
			return _siteRepository.GetById(siteId);
		}

		public IEnumerable<PageDto> GetPagesFor(Guid siteId)
		{
			return _siteRepository.GetById(siteId).Pages;
		}

		public CrawlSpec GetCrawlInfoFor(Guid siteId)
		{
			return new CrawlSpec(_siteRepository.GetById(siteId));
		}

		#endregion
	}

	public class CrawlSpec
	{
		public Guid SiteId { get; set; }
		public string Root { get; set; }

		private readonly List<PageInfo> _pageInfo;
		public IEnumerable<PageInfo> Pages { get { return _pageInfo; } }

		public CrawlSpec(SiteDto site)
		{
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