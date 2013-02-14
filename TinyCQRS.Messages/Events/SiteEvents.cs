using System;
using System.Collections.Generic;
using TinyCQRS.Contracts.Models;

namespace TinyCQRS.Contracts.Events
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

	public class NewPagesAdded : Event
	{
		public IEnumerable<Guid> AddedPages { get; set; }
		public DateTime TimeOfAddition { get; set; }

		public NewPagesAdded(Guid siteId, IEnumerable<Guid> addedPages, DateTime timeOfAddition) : base(siteId)
		{
			AddedPages = addedPages;
			TimeOfAddition = timeOfAddition;
		}
	}

	public class ExistingPagesUpdated : Event
	{
		public IEnumerable<Guid> UpdatedPages { get; set; }
		public DateTime TimeOfModification { get; set; }

		public ExistingPagesUpdated(Guid siteId, IEnumerable<Guid> updatedPages, DateTime timeOfModification) : base(siteId)
		{
			UpdatedPages = updatedPages;
			TimeOfModification = timeOfModification;
		}
	}

	public class ExistingPagesRemoved : Event
	{
		public IEnumerable<Guid> RemovedPages { get; set; }
		public DateTime TimeOfRemoval { get; set; }

		public ExistingPagesRemoved(Guid siteId, IEnumerable<Guid> removedPages, DateTime timeOfRemoval) : base(siteId)
		{
			RemovedPages = removedPages;
			TimeOfRemoval = timeOfRemoval;
		}
	}

	public class PageComponentsIdentified : Event
	{
		public Guid PageId { get; set; }
		public PageComponents PageComponents { get; set; }

		public PageComponentsIdentified(Guid siteId, Guid pageId, PageComponents pageComponents) : base(siteId)
		{
			PageComponents = pageComponents;
			PageId = pageId;
		}
	}
}