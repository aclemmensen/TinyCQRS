using System;
using System.Collections.Generic;

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

	public class SpellcheckCompleted : Event
	{
		public SpellcheckCompleted(Guid pageId) : base(pageId)
		{
		}
	}

	public class CrawlCompleted : Event
	{
		public Guid SiteId { get; set; }
		public DateTime TimeOfCompletion { get; set; }
		public int PagesProcessed { get; set; }

		public IEnumerable<Guid> NewPages { get; set; }
		public IEnumerable<Guid> ChangedPages { get; set; }
		public IEnumerable<Guid> UnchangedPages { get; set; }
		public IEnumerable<Guid> MissingPages { get; set; }

		public CrawlCompleted(
			Guid crawlId,
			Guid siteId,
			DateTime timeOfCompletion,
			int pagesProcessed,
			IEnumerable<Guid> newPages,
			IEnumerable<Guid> changedPages,
			IEnumerable<Guid> unchangedPages,
			IEnumerable<Guid> missingPages)
			: base(crawlId)
		{
			SiteId = siteId;
			TimeOfCompletion = timeOfCompletion;
			PagesProcessed = pagesProcessed;
			NewPages = newPages;
			ChangedPages = changedPages;
			UnchangedPages = unchangedPages;
			MissingPages = missingPages;
		}
	}
}