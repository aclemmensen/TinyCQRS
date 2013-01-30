using System;

namespace TinyCQRS.Messages.Events
{
	public class SiteCreatedEvent : Event
	{
		public string Name { get; set; }
		public string Root { get; private set; }

		public SiteCreatedEvent(Guid id, string name, string root)
			: base(id)
		{
			Name = name;
			Root = root;
		}
	}

	public class PageAddedEvent : Event
	{
		public Guid PageId { get; private set; }

		public PageAddedEvent(Guid siteId, Guid pageId) : base(siteId)
		{
			PageId = pageId;
		}
	}
}