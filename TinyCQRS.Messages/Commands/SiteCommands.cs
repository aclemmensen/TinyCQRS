using System;

namespace TinyCQRS.Messages.Commands
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

	public class CreateNewPage : Command
	{
		public Guid PageId { get; set; }
		public string Url { get; private set; }
		public string Content { get; private set; }

		public CreateNewPage(Guid siteId, Guid pageId, string url, string content)
			: base(siteId)
		{
			PageId = pageId;
			Url = url;
			Content = content;
		}
	}

	public class UpdatePageContent : Command
	{
		public Guid PageId { get; set; }
		public string NewContent { get; private set; }

		public UpdatePageContent(Guid siteId, Guid pageId, string newContent)
			: base(siteId)
		{
			PageId = pageId;
			NewContent = newContent;
		}
	}
}