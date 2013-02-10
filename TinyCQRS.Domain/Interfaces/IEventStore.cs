using System;
using System.Collections.Generic;
using TinyCQRS.Contracts;

namespace TinyCQRS.Domain.Interfaces
{
    public interface IEventStore<T> where T : IEventSourced
    {
		int Processed { get; }

	    IEnumerable<Event> GetEventsFor(Guid id);
	    Event GetLastEventFor(Guid id);
	    void StoreEvent(Event @event);
    }
}