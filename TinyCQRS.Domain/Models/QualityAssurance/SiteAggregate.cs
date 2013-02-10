using System;
using System.Collections.Generic;
using System.Linq;
using TinyCQRS.Contracts.Events;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Domain.Models.QualityAssurance
{
	public class SiteAggregate : AggregateRoot,
		IApply<SiteCreatedEvent>,
		IApply<NewPagesAdded>,
		IApply<ExistingPagesUpdated>,
		IApply<ExistingPagesRemoved>
	{
		private string _root;
		private string _name;
		private readonly HashSet<Guid> _pages = new HashSet<Guid>();

		public SiteAggregate() { }

		public SiteAggregate(Guid id, string name, string root)
		{
			ApplyChange(new SiteCreatedEvent(id, name, root));
		}

		public void AddPages(IEnumerable<Guid> pageIds, DateTime time)
		{
			var newPages = pageIds as IList<Guid> ?? pageIds.ToList();
			if (newPages.Any(_pages.Contains))
			{
				throw new InvalidOperationException("Cannot add existing page to site");
			}

			ApplyChange(new NewPagesAdded(_id, newPages, time));
		}

		public void UpdatePages(IEnumerable<Guid> pageIds, DateTime time)
		{
			var updatedPages = pageIds as IList<Guid> ?? pageIds.ToList();
			if (updatedPages.Any(x => !_pages.Contains(x)))
			{
				throw new InvalidOperationException("Cannot update a page that hasn't been added before.");
			}

			ApplyChange(new ExistingPagesUpdated(_id, updatedPages, time));
		}

		public void RemovePages(IEnumerable<Guid> pageIds, DateTime time)
		{
			var removedPages = pageIds as IList<Guid> ?? pageIds.ToList();
			if (removedPages.Any(x => !_pages.Contains(x)))
			{
				throw new InvalidOperationException("Cannot remove a page that doesn't exist on the site.");
			}

			ApplyChange(new ExistingPagesRemoved(_id, removedPages, time));
		}

		public void Apply(SiteCreatedEvent @event)
		{
			_id = @event.AggregateId;
			_name = @event.Name;
			_root = @event.Root;
		}

		public void Apply(NewPagesAdded @event)
		{
			foreach (var id in @event.AddedPages)
			{
				_pages.Add(id);
			}
		}

		public void Apply(ExistingPagesUpdated @event)
		{
			
		}

		public void Apply(ExistingPagesRemoved @event)
		{
			foreach (var id in @event.RemovedPages)
			{
				_pages.Remove(id);
			}
		}
	}
}