using System;

namespace TinyCQRS.Messages.Commands
{
	public class StartCrawl : Command
	{
		public Guid SiteId { get; set; }
		public DateTime StartTime { get; private set; }

		public StartCrawl(Guid crawlid, Guid siteId, DateTime startTime) : base(crawlid)
		{
			SiteId = siteId;
			StartTime = startTime;
		}
	}

	public class RegisterPageCheck : Command
	{
		public Guid PageId { get; set; }
		public DateTime TimeOfCheck { get; set; }

		public RegisterPageCheck(Guid crawlId, Guid pageId, DateTime timeOfCheck)
			: base(crawlId)
		{
			PageId = pageId;
			TimeOfCheck = timeOfCheck;
		}
	}
}