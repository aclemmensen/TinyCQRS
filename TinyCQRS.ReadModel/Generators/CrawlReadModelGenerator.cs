﻿using System;
using TinyCQRS.Messages;
using TinyCQRS.Messages.Events;
using TinyCQRS.ReadModel.Interfaces;
using TinyCQRS.ReadModel.Model;
namespace TinyCQRS.ReadModel.Generators
{
	public class CrawlReadModelGenerator :
		IConsume<CrawlStarted>
	{
		private readonly IReadModelRepository<Crawl> _crawls;

		public CrawlReadModelGenerator(IReadModelRepository<Crawl> crawls)
		{
			_crawls = crawls;
		}

		//public void Process(PageChecked @event)
		//{
		//	var page = _pages.Get(@event.AggregateId);

		//	page.Checks.Add(new PageCheck
		//	{
		//		Page = page,
		//		PageId = page.Id,
		//		TimeOfChange = @event.TimeOfChange
		//	});

		//	_pages.Update(page);
		//	_pages.Commit();
		//}

		//public void Process(CrawlStarted @event)
		//{
		//	var site = _sites.Get(@event.AggregateId);

		//	var crawl = _crawls.Create();
		//	crawl.Id = Guid.NewGuid();
		//	crawl.Site = site;
		//	crawl.SiteId = site.Id;
		//	crawl.StartTime = @event.StartTime;

		//	_crawls.Add(crawl);

		//	_crawls.Commit();
		//}

		public void Process(CrawlStarted @event)
		{
			var crawl = _crawls.Create();

			crawl.Id = @event.AggregateId;
			crawl.SiteId = @event.SiteId;
			crawl.StartTime = @event.StartTime;

			_crawls.Add(crawl);
			_crawls.Commit();
		}
	}
}
