using System;
using System.Collections.Generic;
using TinyCQRS.Contracts;

namespace TinyCQRS.Domain.Interfaces
{
    public interface IEventStore
    {
		int Processed { get; }

	    IEnumerable<Event> GetEventsFor(Guid id);
	    Event GetLastEventFor(Guid id);
	    void StoreEvent<TAggregate>(Event @event) where TAggregate : IEventSourced;
    }
}