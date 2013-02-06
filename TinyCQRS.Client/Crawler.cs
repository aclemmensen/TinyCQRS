﻿using System;
using System.Collections.Generic;
using System.Linq;
using TinyCQRS.Messages.Commands;

namespace TinyCQRS.Client
{
	class Crawler
	{
		private readonly ISiteCrawlService _service;
		private readonly ILogger _logger;
		private readonly Dictionary<string,PageInfo> _urlMap = new Dictionary<string, PageInfo>();
		private readonly HashSet<string> _seenUrls = new HashSet<string>();

		private CrawlSpec _spec;
		private Guid _crawlId;

		public Crawler(ISiteCrawlService service, ILogger logger)
		{
			_service = service;
			_logger = logger;
		}

		public void Crawl(Guid siteId)
		{
			_crawlId = Guid.NewGuid();
			_logger.Log("Starting crawl for site {0}", siteId);
			_service.StartCrawl(new StartCrawl(_crawlId, siteId, DateTime.UtcNow));

			_spec = _service.GetCrawlInfoFor(siteId);
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
					_service.UpdatePageContent(new UpdatePageContent(_spec.SiteId, page.PageId, content, DateTime.UtcNow));
				}
				else
				{
					_logger.Log("No change: {0}", url);
					_service.PageCheckedWithoutChanges(new RegisterPageCheck(_spec.SiteId, page.PageId, DateTime.UtcNow));
				}

				_seenUrls.Add(url);
			}
			else
			{
				_logger.Log("Adding new page: {0}", url);
				_service.RegisterNewPage(new RegisterNewPage(_crawlId, Guid.NewGuid(), url, content, DateTime.UtcNow));
				_seenUrls.Add(url);
			}
		}
	}
}