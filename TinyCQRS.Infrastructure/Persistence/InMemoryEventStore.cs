using System;
using System.Linq;
using System.Collections.Generic;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages;

namespace TinyCQRS.Infrastructure.Persistence
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly Dictionary<Guid,List<Event>> _events = new Dictionary<Guid, List<Event>>();

        public IEnumerable<Event> GetEventsFor(Guid id)
        {
            List<Event> events;

            return _events.TryGetValue(id, out events) 
                ? events.OrderBy(x => x.Version).ToList() 
                : new List<Event>();
        }

        public void StoreEvent(Guid id, Event @event)
        {
            List<Event> events;
            if (!_events.TryGetValue(id, out events))
            {
                _events[id] = new List<Event>();
                events = _events[id];
            }

            events.Add(@event);
        }

        public void Dispose()
        {
            _events.Clear();
        }
    }
}