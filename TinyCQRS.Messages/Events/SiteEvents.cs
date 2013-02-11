using System;
using System.Collections.Generic;

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

	public class PageCreated : Event
	{
		public Guid SiteId { get; set; }
		public Guid PageId { get; set; }
		public string Url { get; private set; }
		public BlobReference BlobReference { get; set; }
		public DateTime TimeOfCreation { get; set; }

		public PageCreated(Guid crawlId, Guid siteId, Guid pageId, string url, BlobReference blobReference, DateTime timeOfCreation)
			: base(crawlId)
		{
			SiteId = siteId;
			PageId = pageId;
			Url = url;
			BlobReference = blobReference;
			TimeOfCreation = timeOfCreation;
		}
	}

	public class PageContentChanged : Event
	{
		public Guid PageId { get; set; }
		public BlobReference BlobReference { get; set; }

		public DateTime TimeOfChange { get; set; }

		public PageContentChanged(Guid siteId, Guid pageId, BlobReference blobReference, DateTime timeOfChange)
			: base(siteId)
		{
			PageId = pageId;
			BlobReference = blobReference;
			TimeOfChange = timeOfChange;
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
}