using System;
using System.Collections.Generic;
using System.Linq;

namespace TinyCQRS.Client
{
	class Crawler
	{
		private readonly ISiteCrawlService _service;
		private readonly ILogger _logger;
		private readonly Dictionary<string,PageInfo> _urlMap = new Dictionary<string, PageInfo>();
		private readonly HashSet<string> _seenUrls = new HashSet<string>();

		private CrawlSpec _spec;

		public Crawler(ISiteCrawlService service, ILogger logger)
		{
			_service = service;
			_logger = logger;
		}

		public void Crawl(Guid siteId, Guid crawlId)
		{
			_logger.Log("Starting crawl {0} for site {1}", crawlId, siteId);
			_service.StartCrawl(crawlId, siteId);

			_spec = _service.GetCrawlInfoFor(crawlId);
			_urlMap.Clear();

			foreach (var pageInfo in _spec.Pages.Where(page => !_urlMap.ContainsKey(page.Url)))
			{
				_urlMap.Add(pageInfo.Url, pageInfo);
			}

			_logger.Log("Loaded {0} existing pages", _urlMap.Count);
		}

		public void Handle(string url, string content)
		{
			if (_seenUrls.Contains(url))
			{
				_logger.Log("Ignoring already seen: {0}", url);
				return;
			}

			if (_urlMap.ContainsKey(url))
			{
				var page = _urlMap[url];
				if (!HashingHelper.Hash(content).Equals(page.ContentHash))
				{
					_logger.Log("New content: {0}", url);
					_service.UpdatePageContent(_spec.CrawlId, page.PageId, content);
				}
				else
				{
					_logger.Log("No change: {0}", url);
					_service.PageCheckedWithoutChanges(_spec.CrawlId, page.PageId, DateTime.UtcNow);
				}

				_seenUrls.Add(url);
			}
			else
			{
				_logger.Log("Adding new page: {0}", url);
				_service.AddNewPage(_spec.CrawlId, Guid.NewGuid(), url, content);
				_seenUrls.Add(url);
			}
		}
	}
}