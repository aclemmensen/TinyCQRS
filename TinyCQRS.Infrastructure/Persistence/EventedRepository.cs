using System;
using System.Collections.Generic;
using System.Linq;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Persistence
{
    public class EventedRepository<T> : IRepository<T> where T : AggregateRoot, new()
    {
	    private readonly Dictionary<Guid,AggregateRoot> _cache = new Dictionary<Guid, AggregateRoot>();

        private readonly IEventStore _eventStore;

        public EventedRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public T GetById(Guid id)
        {
			if (!_cache.ContainsKey(id))
			{
				var aggregate = new T();
				aggregate.LoadFrom(_eventStore.GetEventsFor(id));
				_cache[id] = aggregate;
			}
            
            return _cache[id] as T;
        }

        public void Save(T aggregate, int? expectedVersion = null)
        {
            int exp = expectedVersion ?? aggregate.Version;

	        var lastEvent = _eventStore.GetLastEventFor(aggregate.Id);
            if (lastEvent != null && exp != 0 && lastEvent.Version != exp)
            {
                throw new ConcurrencyException(string.Format(
                    "Expected version {0}, got version {1}",
                    exp, 
                    lastEvent.Version));
            }

            var i = (lastEvent == null) ? 0 : lastEvent.Version;
            foreach (var e in aggregate.PendingEvents)
            {
                e.Version = ++i;
                aggregate.Version = e.Version;
                _eventStore.StoreEvent(e);
            }

            aggregate.ClearPendingEvents();

	        _cache.Remove(aggregate.Id);
	        _cache[aggregate.Id] = aggregate;
        }
    }
}