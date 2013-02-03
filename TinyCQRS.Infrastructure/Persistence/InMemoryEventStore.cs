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

	    public int Processed { get { return _processed; } }
	    private int _processed;

	    public IEnumerable<Event> GetEventsFor(Guid id)
        {
			var events = new List<Event>();

            if (_events.ContainsKey(id))
            {
	            events = _events[id];
            }

		    Console.WriteLine("Fetching {0} events", events.Count);

		    return events;
        }

		public Event GetLastEventFor(Guid id)
		{
			return GetEventsFor(id).LastOrDefault();
		}

        public void StoreEvent(Event @event)
        {
	        _processed++;

            if (!_events.ContainsKey(@event.AggregateId))
            {
                _events[@event.AggregateId] = new List<Event>();
            }

            _events[@event.AggregateId].Add(@event);
        }

        public void Dispose()
        {
            _events.Clear();
        }
    }
}