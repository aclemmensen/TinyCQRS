using System;
using System.Collections.Generic;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages;

namespace TinyCQRS.Infrastructure.Persistence
{
    public class DispatchingEventStore : IEventStore
    {
        private readonly IEventStore _eventStore;
        private readonly IMessageBus _bus;

        public DispatchingEventStore(IEventStore eventStore, IMessageBus bus)
        {
            _eventStore = eventStore;
            _bus = bus;
        }

        public void Dispose()
        {
            _eventStore.Dispose();
        }

        public IEnumerable<Event> GetEventsFor(Guid id)
        {
            return _eventStore.GetEventsFor(id);
        }

        public void StoreEvent(Guid id, Event @event)
        {
            _eventStore.StoreEvent(id, @event);
            _bus.Notify(@event);
        }
    }
}