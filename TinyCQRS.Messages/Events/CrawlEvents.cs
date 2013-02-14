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

	public class PageCreated : Event
	{
		public Guid SiteId { get; set; }
		public Guid PageId { get; set; }
		public string Url { get; private set; }
		public BlobReference BlobReference { get; set; }
		public DateTime TimeOfCreation { get; set; }

		public PageCreated(Guid crawlId, Guid siteId, Guid pageId, string url, BlobReference blobReference, DateTime timeOfCreation)
			: base(crawlId)
		{
			SiteId = siteId;
			PageId = pageId;
			Url = url;
			BlobReference = blobReference;
			TimeOfCreation = timeOfCreation;
		}
	}

	public class PageContentChanged : Event
	{
		public Guid PageId { get; set; }
		public BlobReference BlobReference { get; set; }

		public DateTime TimeOfChange { get; set; }

		public PageContentChanged(Guid siteId, Guid pageId, BlobReference blobReference, DateTime timeOfChange)
			: base(siteId)
		{
			PageId = pageId;
			BlobReference = blobReference;
			TimeOfChange = timeOfChange;
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