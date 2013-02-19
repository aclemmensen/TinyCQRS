using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TinyCQRS.Contracts;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Caching
{
	public class CachingEventStore : IEventStore
	{
		private readonly IEventStore _innerEventStore;
		private readonly ConcurrentDictionary<Guid, List<Event>> _data = new ConcurrentDictionary<Guid, List<Event>>();
		private readonly ConcurrentDictionary<Guid, int> _lastVersion = new ConcurrentDictionary<Guid, int>();

		public int Processed { get { return _processed; } }
		private int _processed;

		public CachingEventStore(IEventStore innerEventStore)
		{
			_innerEventStore = innerEventStore;
		}

		public IEnumerable<Event> GetEventsFor<T>(Guid id) where T : IEventSourced
		{
			return Get<T>(id);
		}

		public int GetVersionFor<T>(Guid id) where T : IEventSourced
		{
			int version;
			return !_lastVersion.TryGetValue(id, out version) ? 0 : version;
		}

		public void StoreEvent<TAggregate>(Event @event) where TAggregate : IEventSourced
		{
			Interlocked.Increment(ref _processed);
			_innerEventStore.StoreEvent<TAggregate>(@event);
			_lastVersion[@event.AggregateId] = @event.Version;

			Add<TAggregate>(@event);
		}

		private void Add<T>(Event @event)
		{
			if (!_data.ContainsKey(@event.AggregateId))
			{
				_data[@event.AggregateId] = new List<Event>();
			}

			_data[@event.AggregateId].Add(@event);
		}

		private IEnumerable<Event> Get<T>(Guid id) where T : IEventSourced
		{
			if (!_data.ContainsKey(id))
			{
				_data[id] = new List<Event>(_innerEventStore.GetEventsFor<T>(id));
			}

			return _data[id];
		}
	}
}