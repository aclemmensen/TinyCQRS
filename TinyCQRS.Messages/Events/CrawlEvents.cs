using System;

namespace TinyCQRS.Messages.Events
{
	public class CrawlStarted : Event
	{
		public Guid SiteId { get; set; }
		public DateTime StartTime { get; set; }

		public CrawlStarted(Guid crawlId, Guid siteId, DateTime startTime) : base(crawlId)
		{
			SiteId = siteId;
			StartTime = startTime;
		}
	}

	public class PageChecked : Event
	{
		public Guid PageId { get; set; }

		public DateTime TimeOfCheck { get; set; }

		public PageChecked(Guid crawlId, Guid pageId, DateTime timeOfCheck)
			: base(crawlId)
		{
			PageId = pageId;
			TimeOfCheck = timeOfCheck;
		}
	}
}