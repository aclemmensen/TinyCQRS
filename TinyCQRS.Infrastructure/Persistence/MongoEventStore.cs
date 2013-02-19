using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TinyCQRS.Contracts;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class MongoEventStore : IEventStore
	{
		private readonly MongoDatabase _db;
		private readonly Dictionary<Type, MongoCollection<EventEvenlope>> _collections = new Dictionary<Type, MongoCollection<EventEvenlope>>();

		public int Processed { get; private set; }

		public MongoEventStore() : this("mongodb://192.168.10.91") { }

		public MongoEventStore(string connstr)
		{
			var client = new MongoClient(connstr);
			var server = client.GetServer();
			_db = server.GetDatabase("storage");

			if (!BsonClassMap.IsClassMapRegistered(typeof (Message)))
			{
				BsonClassMap.RegisterClassMap<Message>(x =>
				{
					x.AutoMap();
					x.UnmapProperty(y => y.AggregateId);
					x.UnmapProperty(y => y.CorrelationId);
				});

				BsonClassMap.RegisterClassMap<Event>(x =>
				{
					x.AutoMap();
					x.UnmapProperty(y => y.Version);
				});
			}
		}

		public IEnumerable<Event> GetEventsFor<T>(Guid id) where T : IEventSourced
		{
			return Collection<T>()
				.Find(Query<EventEvenlope>.EQ(x => x.AggregateId, id))
				.SetSortOrder(SortBy<EventEvenlope>.Ascending(x => x.Version))
				.Select(x => x.RehydratedEvent);
		}

		public int GetVersionFor<T>(Guid id) where T : IEventSourced
		{
			var envelope = Collection<T>()
				.Find(Query<EventEvenlope>.EQ(x => x.AggregateId, id))
				.SetSortOrder(SortBy<EventEvenlope>.Descending(x => x.Version))
				.SetLimit(1)
				.SetFields(new[] {"Version"});

			var e = envelope.FirstOrDefault();

			return e != null
				? e.Version
				: 0;
		}

		public void StoreEvent<T>(Event @event) where T : IEventSourced
		{
			Processed++;
			Collection<T>().Insert(new EventEvenlope(@event));
		}

		private readonly object _collectionLock = new object();

		private MongoCollection<EventEvenlope> Collection<T>() where T : IEventSourced
		{
			var type = typeof (T);

			MongoCollection<EventEvenlope> collection;
			if (!_collections.TryGetValue(type, out collection))
			{
				lock (_collectionLock)
				{
					collection = _db.GetCollection<EventEvenlope>("Events." + type.Name, WriteConcern.Unacknowledged);
					collection.EnsureIndex(new IndexKeysBuilder().Ascending("AggregateId", "Version"), new IndexOptionsBuilder().SetName("Aggregate_Version"));
					if (!_collections.ContainsKey(type))
					{
						_collections.Add(type, collection);
					}
				}
			}

			return collection;
		}

		public class EventEvenlope
		{
			public ObjectId Id { get; set; }
			public Guid AggregateId { get; set; }
			public Guid CorrelationId { get; set; }
			public DateTime Created { get; set; }
			public int Version { get; set; }
			public Event Event { get; set; }

			[BsonIgnore]
			public Event RehydratedEvent
			{
				get
				{
					var e = Event;

					e.AggregateId = AggregateId;
					e.CorrelationId = CorrelationId;
					e.Version = Version;

					return e;
				}
			}

			public EventEvenlope() { }

			public EventEvenlope(Event @event) 
			{
				Created = DateTime.UtcNow;
				AggregateId = @event.AggregateId;
				CorrelationId = @event.CorrelationId;
				Version = @event.Version;
				Event = @event;
			}
		}
	}
}