using System;
using System.Collections.Generic;
using System.Linq;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages;

namespace TinyCQRS.Infrastructure.Caching
{
	public class CachingEventStore : IEventStore
	{
		private readonly IEventStore _innerEventStore;
		private readonly ICache<IList<Event>> _cache;

		public int Processed { get; private set; }

		public CachingEventStore(IEventStore innerEventStore, ICache<IList<Event>> cache)
		{
			_innerEventStore = innerEventStore;
			_cache = cache;
		}

		public IEnumerable<Event> GetEventsFor(Guid id)
		{
			return GetEventCollectionFor(id);
		}

		public Event GetLastEventFor(Guid id)
		{
			return GetEventCollectionFor(id).LastOrDefault();
		}

		public void StoreEvent(Event @event)
		{
			Processed++;

			_innerEventStore.StoreEvent(@event);

			var existing = GetEventCollectionFor(@event.AggregateId);
			existing.Add(@event);
			_cache.Set(@event.AggregateId, existing);
		}

		private IList<Event> GetEventCollectionFor(Guid id)
		{
			return _cache.Get(id, () => _innerEventStore.GetEventsFor(id).ToList());
		}
	}
}