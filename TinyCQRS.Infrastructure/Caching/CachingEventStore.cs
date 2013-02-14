using System;
using System.Collections.Generic;
using System.Linq;
using TinyCQRS.Contracts;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Caching
{
	public class CachingEventStore : IEventStore
	{
		private readonly IEventStore _innerEventStore;
		private readonly Dictionary<Guid,List<Event>> _data = new Dictionary<Guid, List<Event>>();
		private readonly Dictionary<Guid,Event> _lastEvent = new Dictionary<Guid, Event>();

		public int Processed { get; private set; }

		public CachingEventStore(IEventStore innerEventStore)
		{
			_innerEventStore = innerEventStore;
		}

		public IEnumerable<Event> GetEventsFor(Guid id)
		{
			return Get(id);
		}

		public Event GetLastEventFor(Guid id)
		{
			Event e;
			return !_lastEvent.TryGetValue(id, out e) ? null : e;
		}

		public void StoreEvent<TAggregate>(Event @event) where TAggregate : IEventSourced
		{
			Processed++;

			_innerEventStore.StoreEvent<TAggregate>(@event);
			_lastEvent[@event.AggregateId] = @event;

			Add(@event);
		}

		private void Add(Event @event)
		{
			if (!_data.ContainsKey(@event.AggregateId))
			{
				_data[@event.AggregateId] = new List<Event>();
			}

			_data[@event.AggregateId].Add(@event);
		}

		private IEnumerable<Event> Get(Guid id)
		{
			if (!_data.ContainsKey(id))
			{
				_data[id] = new List<Event>(_innerEventStore.GetEventsFor(id));
			}

			return _data[id];
		}
	}
}