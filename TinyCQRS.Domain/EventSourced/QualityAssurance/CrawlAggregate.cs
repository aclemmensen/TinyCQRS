using System;
using System.Collections.Generic;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages.Events;

namespace TinyCQRS.Domain.EventSourced.QualityAssurance
{
	public class CrawlAggregate : AggregateRoot,
		IApply<CrawlStarted>,
		IApply<PageChecked>,
		IApply<PageCreated>,
		IApply<PageContentChanged>
	{
		private Guid _siteId;
		private DateTime _startTime;

		public CrawlAggregate() { }

		public CrawlAggregate(Guid crawlId, Guid siteId, DateTime startTime)
		{
			ApplyChange(new CrawlStarted(crawlId, siteId, startTime));
		}

		public void RegisterNewPage(Guid pageId, string url, string content, DateTime timeOfCreation)
		{
			ApplyChange(new PageCreated(_id, _siteId, pageId, url, content, timeOfCreation));
		}

		public void RegisterPageCheck(Guid pageId, DateTime timeOfCheck)
		{
			ApplyChange(new PageChecked(_id, pageId, timeOfCheck));
		}

		public void RegisterPageContentChange(Guid pageId, string newContent, DateTime timeOfChange)
		{
			ApplyChange(new PageContentChanged(_id, pageId, newContent, timeOfChange));
		}

		public void Apply(CrawlStarted @event)
		{
			_id = @event.AggregateId;
			_siteId = @event.SiteId;
			_startTime = @event.StartTime;
		}

		public void Apply(PageChecked @event)
		{
			
		}

		public void Apply(PageCreated @event)
		{
			
		}

		public void Apply(PageContentChanged @event)
		{
			
		}
	}
}