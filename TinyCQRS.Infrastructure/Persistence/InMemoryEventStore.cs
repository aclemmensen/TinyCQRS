using System;
using System.Linq;
using System.Collections.Generic;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages;

namespace TinyCQRS.Infrastructure.Persistence
{
    public class InMemoryEventStore : IEventStore, IDisposable
    {
        private readonly Dictionary<Guid,List<Event>> _events = new Dictionary<Guid, List<Event>>();

        public IEnumerable<Event> GetEventsFor(Guid id)
        {
            List<Event> events;

            return _events.TryGetValue(id, out events) 
                ? events.OrderBy(x => x.Version).ToList() 
                : new List<Event>();
        }

		public Event GetLastEventFor(Guid id)
		{
			return GetEventsFor(id).LastOrDefault();
		}

        public void StoreEvent(Event @event)
        {
            List<Event> events;
            if (!_events.TryGetValue(@event.AggregateId, out events))
            {
                _events[@event.AggregateId] = new List<Event>();
				events = _events[@event.AggregateId];
            }

            events.Add(@event);
        }

        public void Dispose()
        {
            _events.Clear();
        }
    }
}