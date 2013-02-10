using System;
using System.Collections.Generic;
using TinyCQRS.Contracts;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Persistence
{
    public class DispatchingEventStore<T> : IEventStore<T> where T : IEventSourced
    {
        private readonly IEventStore<T> _eventStore;
        private readonly IMessageBus _bus;

		public int Processed { get { return _eventStore.Processed; } }

        public DispatchingEventStore(IEventStore<T> eventStore, IMessageBus bus)
        {
            _eventStore = eventStore;
            _bus = bus;
        }

        public void Dispose()
        {
	        var disposable = _eventStore as IDisposable;
			
			if (disposable != null)
			{
				disposable.Dispose();
			}
        }


	    public IEnumerable<Event> GetEventsFor(Guid id)
        {
            return _eventStore.GetEventsFor(id);
        }

	    public Event GetLastEventFor(Guid id)
	    {
		    return _eventStore.GetLastEventFor(id);
	    }

	    public void StoreEvent(Event @event)
        {
            _eventStore.StoreEvent(@event);
            _bus.Notify(@event);
        }
    }
}