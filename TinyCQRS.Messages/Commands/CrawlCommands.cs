using System;
using System.Collections.Generic;

namespace TinyCQRS.Contracts.Commands
{
	public class StartCrawl : Command
	{
		public string CrawlerName { get; set; }
		public DateTime StartTime { get; private set; }

		public StartCrawl(Guid crawlid, string crawlerName, DateTime startTime) : base(crawlid)
		{
			CrawlerName = crawlerName;
			StartTime = startTime;
		}
	}

	public class RegisterCheckWithoutChange : Command
	{
		public Guid PageId { get; set; }
		public DateTime TimeOfCheck { get; set; }

		public RegisterCheckWithoutChange(Guid crawlId, Guid pageId, DateTime timeOfCheck)
			: base(crawlId)
		{
			PageId = pageId;
			TimeOfCheck = timeOfCheck;
		}
	}

	public class OrderCrawl : Command
	{
		public Guid SiteId { get; set; }
		public DateTime TimeOfOrder { get; set; }

		public OrderCrawl(Guid crawlId, Guid siteId, DateTime timeOfOrder) : base(crawlId)
		{
			SiteId = siteId;
			TimeOfOrder = timeOfOrder;
		}
	}

	public class OrderPageCheck : Command
	{
		public Guid PageId { get; set; }
		public DateTime TimeOfOrder { get; set; }

		public OrderPageCheck(Guid siteId, Guid pageId, DateTime timeOfOrder) : base(siteId)
		{
			PageId = pageId;
			TimeOfOrder = timeOfOrder;
		}
	}

	public class RegisterNewPage : Command
	{
		public Guid PageId { get; set; }
		public string Url { get; private set; }
		public string Content { get; private set; }
		public DateTime TimeOfCreation { get; set; }

		public RegisterNewPage(Guid crawlId, Guid pageId, string url, string content, DateTime timeOfCreation)
			: base(crawlId)
		{
			PageId = pageId;
			Url = url;
			Content = content;
			TimeOfCreation = timeOfCreation;
		}
	}

	public class RegisterPageContentChange : Command
	{
		public Guid PageId { get; set; }
		public string NewContent { get; private set; }
		public DateTime TimeOfChange { get; set; }

		public RegisterPageContentChange(Guid crawlId, Guid pageId, string newContent, DateTime timeOfChange)
			: base(crawlId)
		{
			PageId = pageId;
			NewContent = newContent;
			TimeOfChange = timeOfChange;
		}
	}

	public class MarkCrawlComplete : Command
	{
		public DateTime TimeOfCompletion { get; set; }
		public IEnumerable<Guid> MissingPages { get; set; }

		public MarkCrawlComplete(Guid crawlId, DateTime timeOfCompletion, IEnumerable<Guid> missingPages)
			: base(crawlId)
		{
			TimeOfCompletion = timeOfCompletion;
			MissingPages = missingPages;
		}
	}

}