using System;
using System.Collections.Generic;
using TinyCQRS.Contracts;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

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


	    public IEnumerable<Event> GetEventsFor<T>(Guid id) where T : IEventSourced
        {
            return _eventStore.GetEventsFor<T>(id);
        }

	    public int GetVersionFor<T>(Guid id) where T : IEventSourced
	    {
		    return _eventStore.GetVersionFor<T>(id);
	    }

	    public void StoreEvent<T>(Event @event) where T : IEventSourced
        {
            _eventStore.StoreEvent<T>(@event);
            _bus.Notify(@event);
        }
    }
}