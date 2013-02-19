using System;
using System.Collections.Generic;
using TinyCQRS.Contracts;

namespace TinyCQRS.Domain.Interfaces
{
    public interface IEventStore
    {
		int Processed { get; }

	    IEnumerable<Event> GetEventsFor<T>(Guid id) where T : IEventSourced;
	    int GetVersionFor<T>(Guid id) where T : IEventSourced;
	    void StoreEvent<T>(Event @event) where T : IEventSourced;
    }
}