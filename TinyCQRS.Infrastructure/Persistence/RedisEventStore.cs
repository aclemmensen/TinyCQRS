using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ServiceStack.Redis;
using TinyCQRS.Contracts;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class RedisEventStore : IEventStore
	{
		private readonly PooledRedisClientManager _mgr;

		public int Processed { get { return _processed; } }
		private int _processed;

		public RedisEventStore()
		{
			_mgr = new PooledRedisClientManager(
				//"192.168.2.91"
				//"192.168.10.91"
				"localhost"
				);
			//_mgr = new PooledRedisClientManager("192.168.10.91");
			//_mgr = new PooledRedisClientManager("localhost");
			//_mgr = new PooledRedisClientManager("192.168.137.170");
		}

		public IEnumerable<Event> GetEventsFor<T>(Guid id) where T : IEventSourced
		{
			using (var redis = _mgr.GetClient())
			{
				var events = redis.As<EventEnvelope>().Lists[CollectionKey<T>(id)].Select(x => x.Event);
				return events;
			}
		}

		public int GetVersionFor<T>(Guid id) where T : IEventSourced
		{
			using (var redis = _mgr.GetClient())
			{
				int version;
				return (int.TryParse(redis.GetValue(VersionKey<T>(id)), out version)) ? version : 0;
			}
		}

		public void StoreEvent<T>(Event @event) where T : IEventSourced
		{
			Interlocked.Increment(ref _processed);

			using (var redis = _mgr.GetClient())
			{
				var p = redis.CreatePipeline();
				p.QueueCommand(x => x.IncrementValue(VersionKey<T>(@event.AggregateId)));
				p.QueueCommand(x => x.As<EventEnvelope>().Lists[CollectionKey<T>(@event.AggregateId)].Append(new EventEnvelope(@event)));
				p.Flush();
			}
		}

		private static string VersionKey<T>(Guid id)
		{
			return string.Format("urn.aggregate.{0}.{1}", typeof (T).Name, id).ToLower();
		}

		private static string CollectionKey<T>(Guid id)
		{
			return string.Format("urn.events.{0}.{1}", typeof (T).Name, id).ToLower();
		}

		public class EventEnvelope
		{
			public long Id { get; set; }
			public Guid AggregateId { get; set; }
			public int Version { get; set; }
			public DateTime Created { get; set; }

			public Event Event { get; set; }

			public EventEnvelope() { }

			public EventEnvelope(Event @event)
			{
				AggregateId = @event.AggregateId;
				Version = @event.Version;
				Created = DateTime.UtcNow;

				Event = @event;
			}
		}
	}
}