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
		public Guid PageId { get; set; }
		public string Url { get; private set; }
		public string Content { get; private set; }

		public PageCreated(Guid siteId, Guid pageId, string url, string content)
			: base(siteId)
		{
			PageId = pageId;
			Url = url;
			Content = content;
		}
	}

	public class ResourceAdded : Event
	{
		public Guid ResourceId { get; set; }

		public ResourceAdded(Guid id, Guid resourceId)
			: base(id)
		{
			ResourceId = resourceId;
		}
	}

	public class PageContentChanged : Event
	{
		public string Content { get; private set; }

		public DateTime TimeOfChange { get; set; }

		public PageContentChanged(Guid pageId, string content, DateTime timeOfChange)
			: base(pageId)
		{
			Content = content;
			TimeOfChange = timeOfChange;
		}
	}
}