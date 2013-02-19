using System;
using System.Collections.Generic;
using System.Linq;
using TinyCQRS.Contracts;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Events;
using TinyCQRS.Contracts.Models;
using TinyCQRS.Contracts.Services;
using TinyCQRS.Domain;
using TinyCQRS.Infrastructure;

namespace TinyCQRS.Application.Modules.Crawler
{
	public class Crawler : IConsume<CrawlOrdered>
	{
		private readonly ICrawlService _service;
		private readonly ILogger _logger;
		private readonly Dictionary<string,PageInfo> _urlMap = new Dictionary<string, PageInfo>();
		private readonly HashSet<string> _seenUrls = new HashSet<string>();

		private CrawlSpec _spec;
		private Guid _crawlId;

		public Crawler(ICrawlService service, ILogger logger)
		{
			_service = service;
			_logger = logger;
		}

		public void Crawl(Guid crawlId, Guid siteId)
		{
			_crawlId = crawlId;
			_logger.Log("Starting crawl for site {0}", siteId);
			_service.StartCrawl(new StartCrawl(_crawlId, "MemoryCrawler", DateTime.UtcNow));

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
				return;
			}

			if (_urlMap.ContainsKey(url))
			{
				var page = _urlMap[url];
				if (!HashingHelper.Hash(content).Equals(page.ContentHash))
				{
					_service.UpdatePageContent(new RegisterPageContentChange(_crawlId, page.PageId, content, DateTime.UtcNow));
				}
				else
				{
					_service.PageCheckedWithoutChanges(new RegisterCheckWithoutChange(_crawlId, page.PageId, DateTime.UtcNow));
				}
			}
			else
			{
				_service.RegisterNewPage(new RegisterNewPage(_crawlId, Guid.NewGuid(), url, content, DateTime.UtcNow));
			}

			_seenUrls.Add(url);
		}

		public void Done()
		{
			var missing = _urlMap.Where(x => !_seenUrls.Contains(x.Key)).Select(x => x.Value.PageId).ToList();

			_service.MarkCrawlComplete(new MarkCrawlComplete(_crawlId, DateTime.UtcNow, missing));
		}

		public void Process(CrawlOrdered @event)
		{
			Crawl(@event.AggregateId, @event.SiteId);
			
			var r = new Random();
			var lower = r.Next(1, 1000);
			var upper = lower + 7000;
			var lucky = r.Next(upper, upper + 1000);

			for (var i = lower; i < upper; i++)
			{
				Handle(string.Format("http://someurl.dk/page_{0}.html", i), "This is the content potentially this is mistakefulness");
			}

			Handle(string.Format("http://newurl.dk/{0}_new.html", lucky), "this is a random new page");

			Done();
		}
	}
}