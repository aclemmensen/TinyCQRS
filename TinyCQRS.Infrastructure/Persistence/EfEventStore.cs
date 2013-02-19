using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TinyCQRS.Contracts;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Infrastructure.Database;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class EfEventStore : IEventStore
	{
		private readonly EventContext _context;
		private readonly DbSet<MessageEnvelope> _set;

		private readonly Dictionary<Guid, List<Event>> _cache = new Dictionary<Guid, List<Event>>();

		public EfEventStore(EventContext context)
		{
			_context = context;
			_set = _context.Events;
		}

		public int Processed { get; private set; }

		public IEnumerable<Event> GetEventsFor<T>(Guid id) where T : IEventSourced
		{
			if (!_cache.ContainsKey(id))
			{
				_cache[id] = new List<Event>(
					_set
						.Where(x => x.AggregateId == id)
						.OrderBy(x => x.Version)
						.ToList()
						.Select(x => x.Event));
			}

			return _cache[id];
		}

		public int GetVersionFor<T>(Guid id) where T : IEventSourced
		{
			if (_cache.ContainsKey(id))
			{
				var lastOrDefault = _cache[id].LastOrDefault();
				if (lastOrDefault != null) return lastOrDefault.Version;
			}

			var orDefault = GetEventsFor<T>(id).LastOrDefault();
			if (orDefault != null) return orDefault.Version;
			return 0;
		}

		public void StoreEvent<TAggregate>(Event @event) where TAggregate : IEventSourced
		{
			Processed++;

			if (!_cache.ContainsKey(@event.AggregateId))
			{
				_cache[@event.AggregateId] = new List<Event>();
			}

			_cache[@event.AggregateId].Add(@event);

			_set.Add(new MessageEnvelope(@event));

			_context.SaveChanges();
		}
	}
}