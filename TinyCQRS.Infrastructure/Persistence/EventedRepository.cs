using System;
using System.Collections.Generic;
using System.Linq;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Infrastructure.Caching;

namespace TinyCQRS.Infrastructure.Persistence
{
    public class EventedRepository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        private readonly IEventStore _eventStore;
	    private readonly ICache<AggregateRoot> _cache;

	    public EventedRepository(IEventStore eventStore, ICache<AggregateRoot> cache)
        {
	        _eventStore = eventStore;
	        _cache = cache;
        }

	    public T GetById(Guid id)
	    {
		    return _cache.Get(id, () =>
		    {
				Console.WriteLine("Getting aggregate {0} id {1}", typeof(T).Name, id);

				var aggregate = new T();
				aggregate.LoadFrom(_eventStore.GetEventsFor(id));
				return aggregate;
		    }) as T;
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

	        _cache.Set(aggregate.Id, aggregate);
        }
    }
}