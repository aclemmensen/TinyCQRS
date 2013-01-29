using System;
using System.Collections.Generic;
using TinyCQRS.Messages;

namespace TinyCQRS.Domain.Interfaces
{
    public interface IEventStore : IDisposable
    {
        IEnumerable<Event> GetEventsFor(Guid id);
        void StoreEvent(Guid id, Event @event);
    }
}