using System;
using System.Collections.Generic;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages;
using TinyCQRS.Messages.Events;

namespace TinyCQRS.Domain.EventSourced.QualityAssurance
{
	public class PageAggregate : AggregateRoot, 
		IApply<PageCreated>, 
		IApply<ResourceAdded>,
		IApply<PageContentChanged>
	{
		private string _url;
		private string _content;
		private readonly List<Guid> _resources = new List<Guid>();

		public PageAggregate() { }

		public PageAggregate(Guid id, string url, string content)
		{
			ApplyChange(new PageCreated(id, url, content));
		}

		public void AddResource(Guid resourceId)
		{
			ApplyChange(new ResourceAdded(_id, resourceId));
		}

		public void ChangeContent(string newContent)
		{
			if (_content.Equals(newContent))
			{
				throw new ApplicationException("New content is identical to old content.");
			}

			ApplyChange(new PageContentChanged(_id, newContent));
		}

		public void Apply(PageCreated @event)
		{
			_id = @event.AggregateId;
			_url = @event.Url;
			_content = @event.Content;
		}

		public void Apply(ResourceAdded @event)
		{
			_resources.Add(@event.AggregateId);
		}

		public void Apply(PageContentChanged @event)
		{
			_content = @event.Content;
		}
	}
}