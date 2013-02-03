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

		public int Processed { get { return _eventStore.Processed; } }

        public DispatchingEventStore(IEventStore eventStore, IMessageBus bus)
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