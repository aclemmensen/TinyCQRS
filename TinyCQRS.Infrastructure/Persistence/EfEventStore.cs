using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Infrastructure.Database;
using TinyCQRS.Messages;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class EfEventStore : IEventStore
	{
		private readonly EventContext _context;
		private readonly DbSet<EventEnvelope> _set;

		private readonly Dictionary<Guid, List<Event>> _cache = new Dictionary<Guid, List<Event>>();

		public EfEventStore(EventContext context)
		{
			_context = context;
			_set = _context.Events;
		}

		public IEnumerable<Event> GetEventsFor(Guid id)
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

		public Event GetLastEventFor(Guid id)
		{
			if (_cache.ContainsKey(id))
			{
				return _cache[id].LastOrDefault();
			}

			return GetEventsFor(id).LastOrDefault();
		}

		public void StoreEvent(Event @event)
		{
			if (!_cache.ContainsKey(@event.AggregateId))
			{
				_cache[@event.AggregateId] = new List<Event>();
			}

			_cache[@event.AggregateId].Add(@event);

			_set.Add(new EventEnvelope(@event));

			_context.SaveChanges();
		}
	}
}