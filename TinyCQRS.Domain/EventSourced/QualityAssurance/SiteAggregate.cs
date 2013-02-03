using System;
using System.Collections.Generic;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages.Events;

namespace TinyCQRS.Domain.EventSourced.QualityAssurance
{
	public class SiteAggregate : AggregateRoot,
		IApply<SiteCreatedEvent>,
		IApply<PageCreated>,
		IApply<PageContentChanged>
	{
		private string _root;
		private string _name;
		private readonly HashSet<Guid> _pages = new HashSet<Guid>();

		public SiteAggregate() { }

		public SiteAggregate(Guid id, string name, string root)
		{
			ApplyChange(new SiteCreatedEvent(id, name, root));
		}

		public void AddNewPage(Guid pageId, string url, string content)
		{
			if (_pages.Contains(pageId))
			{
				throw new InvalidOperationException("Page already exists on site");
			}

			ApplyChange(new PageCreated(_id, pageId, url, content));
		}

		public void UpdatePageContent(Guid pageId, string newContent)
		{
			ApplyChange(new PageContentChanged(pageId, newContent, DateTime.UtcNow));
		}

		public void Apply(SiteCreatedEvent @event)
		{
			_id = @event.AggregateId;
			_name = @event.Name;
			_root = @event.Root;
		}

		public void Apply(PageCreated @event)
		{

		}

		public void Apply(PageContentChanged @event)
		{
			
		}
	}
}