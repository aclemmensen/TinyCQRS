using System;

namespace TinyCQRS.Messages.Commands
{
	public class CreatePage : Command
	{
		public string Url { get; private set; }
		public string Content { get; private set; }

		public CreatePage(Guid pageId, string url, string content) : base(pageId)
		{
			Url = url;
			Content = content;
		}
	}

	public class UpdatePageContent : Command
	{
		public string NewContent { get; private set; }

		public UpdatePageContent(Guid pageId, string newContent) : base(pageId)
		{
			NewContent = newContent;
		}
	}
}