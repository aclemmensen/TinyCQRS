using System;
using System.Collections.Generic;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages.Commands;
using TinyCQRS.Messages.Events;

namespace TinyCQRS.Domain.EventSourced.QualityAssurance
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

		public void StartCrawl(Guid crawlId)
		{
			
		}

		public void Apply(SiteCreatedEvent @event)
		{
			_id = @event.AggregateId;
			_name = @event.Name;
			_root = @event.Root;
		}
	}

	public class CrawlProcessManager : IProcessManager
	{
		public Guid Id { get; private set; }
		public bool Complete { get; private set; }

		public Guid SiteId { get; private set; }

		public CrawlProcessManager()
		{
			Id = Guid.NewGuid();
		}

		public CrawlProcessManager(Guid siteId) : this()
		{
			SiteId = siteId;
		}
	}


	public interface IProcessManager
	{
		Guid Id { get; }
		bool Complete { get; }
	}
}