using System;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages.Events;

namespace TinyCQRS.Domain.EventSourced.QualityAssurance
{
	public class CrawlAggregate : AggregateRoot,
		IApply<CrawlStarted>,
		IApply<PageChecked>
	{
		private DateTime _startTime;
		private Guid _siteId;

		public CrawlAggregate() { }

		public CrawlAggregate(Guid crawlId, Guid siteId, DateTime startTime)
		{
			ApplyChange(new CrawlStarted(crawlId, siteId, startTime));
		}

		public void FlagPageAsChecked(Guid pageId, DateTime timeOfCheck)
		{
			ApplyChange(new PageChecked(_id, pageId, timeOfCheck));
		}

		public void Apply(PageChecked @event) { }

		public void Apply(CrawlStarted @event)
		{
			_id = @event.AggregateId;
			_siteId = @event.SiteId;
			_startTime = @event.StartTime;
		}
	}
}