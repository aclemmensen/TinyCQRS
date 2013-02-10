using System;
using System.Collections.Generic;

namespace TinyCQRS.Contracts.Commands
{
	public class CreateNewSite : Command
	{
		public string Name { get; private set; }
		public string Root { get; private set; }

		public CreateNewSite(Guid siteId, string name, string root) : base(siteId)
		{
			Name = name;
			Root = root;
		}
	}

	public class RegisterNewPage : Command
	{
		public Guid PageId { get; set; }
		public string Url { get; private set; }
		public string Content { get; private set; }
		public DateTime TimeOfCreation { get; set; }

		public RegisterNewPage(Guid crawlId, Guid pageId, string url, string content, DateTime timeOfCreation) : base(crawlId)
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

		public RegisterPageContentChange(Guid crawlId, Guid pageId, string newContent, DateTime timeOfChange) : base(crawlId)
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

		public MarkCrawlComplete(Guid crawlId, DateTime timeOfCompletion, IEnumerable<Guid> missingPages) : base(crawlId)
		{
			TimeOfCompletion = timeOfCompletion;
			MissingPages = missingPages;
		}
	}
}