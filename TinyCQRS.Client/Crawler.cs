using System;
using System.Collections.Generic;
using System.Linq;

namespace TinyCQRS.Client
{
	class Crawler
	{
		private readonly ISiteCrawlService _service;
		private readonly Dictionary<string,PageInfo> _urlMap = new Dictionary<string, PageInfo>();
		private readonly HashSet<string> _newUrls = new HashSet<string>();

		private CrawlSpec _spec;

		public Crawler(ISiteCrawlService service)
		{
			_service = service;
		}

		public void Crawl(Guid siteId)
		{
			_spec = _service.GetCrawlInfoFor(siteId);
			_urlMap.Clear();

			foreach (var pageInfo in _spec.Pages.Where(page => !_urlMap.ContainsKey(page.Url)))
			{
				_urlMap.Add(pageInfo.Url, pageInfo);
			}
		}

		public void Handle(string url, string content)
		{
			if (_newUrls.Contains(url))
			{
				return;
			}

			if (_urlMap.ContainsKey(url))
			{
				if (!HashingHelper.Hash(content).Equals(_urlMap[url].ContentHash))
				{
					_service.UpdatePageContent(_spec.SiteId, _urlMap[url].PageId, content);
				}
			}
			else
			{
				_service.AddNewPage(_spec.SiteId, Guid.NewGuid(), url, content);
				_newUrls.Add(url);
			}
		}
	}
}