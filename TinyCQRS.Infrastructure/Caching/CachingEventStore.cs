using System;
using System.Collections.Generic;
using System.Linq;
using TinyCQRS.Contracts;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Caching
{
	public class CachingEventStore<T> : IEventStore<T> where T : IEventSourced
	{
		private readonly IEventStore<T> _innerEventStore;
		private readonly Dictionary<Type,List<Event>> _data = new Dictionary<Type, List<Event>>();

		public int Processed { get; private set; }

		public CachingEventStore(IEventStore<T> innerEventStore)
		{
			_innerEventStore = innerEventStore;
		}

		public IEnumerable<Event> GetEventsFor(Guid id)
		{
			return Get().Where(x => x.AggregateId == id).OrderBy(x => x.Version);
		}

		public Event GetLastEventFor(Guid id)
		{
			return GetEventsFor(id).LastOrDefault();
		}

		public void StoreEvent(Event @event)
		{
			Processed++;

			_innerEventStore.StoreEvent(@event);

			Add(@event);
		}

		private void Add(Event @event)
		{
			if (!_data.ContainsKey(typeof (T)))
			{
				_data[typeof(T)] = new List<Event>();
			}

			_data[typeof(T)].Add(@event);
		}

		private IEnumerable<Event> Get()
		{
			if (!_data.ContainsKey(typeof (T)))
			{
				_data[typeof(T)] = new List<Event>();
			}

			return _data[typeof (T)];
		}
	}
}