using System;

namespace TinyCQRS.Messages.Events
{
	public class SiteCreatedEvent : Event
	{
		public string Name { get; set; }
		public string Root { get; private set; }

		public SiteCreatedEvent(Guid id, string name, string root) : base(id)
		{
			Name = name;
			Root = root;
		}
	}

	public class PageCreated : Event
	{
		public Guid SiteId { get; set; }
		public Guid PageId { get; set; }
		public string Url { get; private set; }
		public string Content { get; private set; }
		public DateTime TimeOfCreation { get; set; }

		public PageCreated(Guid crawlId, Guid siteId, Guid pageId, string url, string content, DateTime timeOfCreation)
			: base(crawlId)
		{
			SiteId = siteId;
			PageId = pageId;
			Url = url;
			Content = content;
			TimeOfCreation = timeOfCreation;
		}
	}

	public class PageContentChanged : Event
	{
		public Guid PageId { get; set; }
		public string Content { get; private set; }

		public DateTime TimeOfChange { get; set; }

		public PageContentChanged(Guid siteId, Guid pageId, string content, DateTime timeOfChange)
			: base(siteId)
		{
			PageId = pageId;
			Content = content;
			TimeOfChange = timeOfChange;
		}
	}
}