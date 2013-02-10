using System;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Persistence
{
    public class EventedAggregateRepository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        private readonly IEventStore<T> _eventStore;
	    private readonly IMessageBus _bus;

	    public EventedAggregateRepository(IEventStore<T> eventStore, IMessageBus bus)
        {
	        _eventStore = eventStore;
		    _bus = bus;
        }

	    public T GetById(Guid id)
	    {
			var obj = new T();
			obj.LoadFrom(_eventStore.GetEventsFor(id));
			return obj;
	    }

        public void Save(T aggregate, int? expectedVersion = null)
        {
            int exp = expectedVersion ?? aggregate.Version;

	        var lastEvent = _eventStore.GetLastEventFor(aggregate.Id);
            if (lastEvent != null && exp != 0 && lastEvent.Version != exp)
            {
                throw new ConcurrencyException(string.Format(
                    "Expected aggregate version {0}, got version {1}",
                    exp, 
                    lastEvent.Version));
            }

            var i = (lastEvent == null) ? 0 : lastEvent.Version;
            foreach (var e in aggregate.PendingEvents)
            {
                e.Version = ++i;
                aggregate.Version = e.Version;
                _eventStore.StoreEvent(e);
				_bus.Notify(e);
            }

            aggregate.ClearPendingEvents();
        }
    }
}