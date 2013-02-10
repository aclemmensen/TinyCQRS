using System;
using System.Collections.Generic;
using TinyCQRS.Contracts.Events;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Domain.Models.QualityAssurance
{
	public class SiteAggregate : AggregateRoot,
		IApply<SiteCreatedEvent>
	{
		private string _root;
		private string _name;
		private readonly HashSet<Guid> _pages = new HashSet<Guid>();

		public SiteAggregate() { }

		public SiteAggregate(Guid id, string name, string root)
		{
			ApplyChange(new SiteCreatedEvent(id, name, root));
		}

		public void Apply(SiteCreatedEvent @event)
		{
			_id = @event.AggregateId;
			_name = @event.Name;
			_root = @event.Root;
		}
	}
}