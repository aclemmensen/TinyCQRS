using System;

namespace TinyCQRS.Messages.Commands
{
	public class StartCrawl : Command
	{
		public Guid SiteId { get; private set; }
		public DateTime StartTime { get; private set; }

		public StartCrawl(Guid crawlId, Guid siteId, DateTime startTime) : base(crawlId)
		{
			SiteId = siteId;
			StartTime = startTime;
		}
	}

	public class RegisterCheck : Command
	{
		public Guid PageId { get; set; }
		public DateTime TimeOfCheck { get; set; }

		public RegisterCheck(Guid crawlId, Guid pageId, DateTime timeOfCheck)
			: base(crawlId)
		{
			PageId = pageId;
			TimeOfCheck = timeOfCheck;
		}
	}
}