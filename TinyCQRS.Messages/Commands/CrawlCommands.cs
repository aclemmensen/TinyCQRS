using System;

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

	public class RegisterNoChangeCheck : Command
	{
		public Guid PageId { get; set; }
		public DateTime TimeOfCheck { get; set; }

		public RegisterNoChangeCheck(Guid crawlId, Guid pageId, DateTime timeOfCheck)
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

	public class SpellcheckPage : Command
	{
		public Guid PageId { get; set; }
		public string NewContent { get; set; }

		public SpellcheckPage(Guid crawlId, Guid pageId, string newContent) : base(crawlId)
		{
			PageId = pageId;
			NewContent = newContent;
		}
	}

	public class ValidatePage : Command
	{
		public Guid PageId { get; set; }
		public string NewContent { get; set; }

		public ValidatePage(Guid crawlId, Guid pageId, string newContent) : base(crawlId)
		{
			PageId = pageId;
			NewContent = newContent;
		}
	}
}