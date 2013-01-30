using System;
using TinyCQRS.Messages.Shared;

namespace TinyCQRS.Messages.Events
{
	public class PageCreated : Event
	{
		public string Url { get; private set; }
		public string Content { get; private set; }

		public PageCreated(Guid id, string url, string content) : base(id)
		{
			Url = url;
			Content = content;
		}
	}

	public class ResourceAdded : Event
	{
		public Guid ResourceId { get; set; }

		public ResourceAdded(Guid id, Guid resourceId) : base(id)
		{
			ResourceId = resourceId;
		}
	}

	public class PageContentChanged : Event
	{
		public string Content { get; private set; }

		public PageContentChanged(Guid pageId, string content) : base(pageId)
		{
			Content = content;
		}
	}
}