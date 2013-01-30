using System;
using System.Collections.Generic;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages.Events;

namespace TinyCQRS.Domain.EventSourced.QualityAssurance
{
	public class SiteAggregate : AggregateRoot,
		IApply<SiteCreatedEvent>,
		IApply<PageAddedEvent>
	{
		private string _root;
		private string _name;
		private readonly HashSet<Guid> _pages = new HashSet<Guid>();

		public SiteAggregate() { }

		public SiteAggregate(Guid id, string name, string root)
		{
			ApplyChange(new SiteCreatedEvent(id, name, root));
		}

		public void AddPage(Guid pageId)
		{
			ApplyChange(new PageAddedEvent(_id, pageId));
		}

		public void Apply(SiteCreatedEvent @event)
		{
			_id = @event.AggregateId;
			_name = @event.Name;
			_root = @event.Root;
		}

		public void Apply(PageAddedEvent @event)
		{
			if (_pages.Contains(@event.PageId))
			{
				throw new ApplicationException("Page already exists");
			}

			_pages.Add(@event.PageId);
		}
	}
}