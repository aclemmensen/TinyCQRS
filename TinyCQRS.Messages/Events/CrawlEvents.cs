using System;

namespace TinyCQRS.Contracts.Events
{
	public class CrawlOrdered : Event
	{
		public Guid SiteId { get; set; }
		public DateTime TimeOfOrder { get; set; }

		public CrawlOrdered(Guid crawlId, Guid siteId, DateTime timeOfOrder) : base(crawlId)
		{
			SiteId = siteId;
			TimeOfOrder = timeOfOrder;
		}
	}

	public class CrawlStarted : Event
	{
		public string CrawlerName { get; set; }
		public DateTime StartTime { get; set; }

		public CrawlStarted(Guid crawlId, string crawlerName, DateTime startTime) : base(crawlId)
		{
			CrawlerName = crawlerName;
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