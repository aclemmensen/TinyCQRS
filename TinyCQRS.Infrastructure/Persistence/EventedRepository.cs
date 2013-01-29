using System;
using System.Linq;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Persistence
{
    public class EventedRepository<T> : IEventedRepository<T> where T : AggregateRoot, new()
    {
        private readonly IEventStore _eventStore;

        public EventedRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public T GetById(Guid id)
        {
            var aggregate = new T();
            aggregate.LoadFrom(_eventStore.GetEventsFor(id));
            return aggregate;
        }

        public void Save(AggregateRoot aggregate, int? expectedVersion = null)
        {
            int exp = expectedVersion ?? aggregate.Version;

            var lastEvent = _eventStore.GetEventsFor(aggregate.Id).LastOrDefault();
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
                _eventStore.StoreEvent(aggregate.Id, e);
            }

            aggregate.ClearPendingEvents();
        }
    }
}