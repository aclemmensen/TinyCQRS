using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using TinyCQRS.Contracts;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Persistence
{
    public class InMemoryEventStore : IEventStore, IDisposable
    {
		private readonly ConcurrentDictionary<Guid, List<Event>> _events = new ConcurrentDictionary<Guid, List<Event>>();

	    public int Processed { get { return _processed; } }
	    private int _processed;

		public IEnumerable<Event> GetEventsFor<T>(Guid id) where T : IEventSourced
        {
			var events = new List<Event>();

            if (_events.ContainsKey(id))
            {
	            events = _events[id];
            }

		    return events;
        }

		public int GetVersionFor<T>(Guid id) where T : IEventSourced
		{
			var last = GetEventsFor<T>(id).LastOrDefault();
			return last == null ? 0 : last.Version;
		}

        public void StoreEvent<TAggregate>(Event @event) where TAggregate : IEventSourced
        {
	        Interlocked.Increment(ref _processed);

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