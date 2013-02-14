using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TinyCQRS.Contracts;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class MongoEventStore : IEventStore
	{
		private readonly MongoCollection<EventEvenlope> _collection;

		public int Processed { get; private set; }

		public MongoEventStore(string connstr)
		{
			var client = new MongoClient(connstr);
			var server = client.GetServer();
			var db = server.GetDatabase("storage");
			_collection = db.GetCollection<EventEvenlope>("Events");
			_collection.EnsureIndex(new IndexKeysBuilder().Ascending("AggregateId"));
		}

		public IEnumerable<Event> GetEventsFor(Guid id)
		{
			return _collection
				.Find(Query<EventEvenlope>.EQ(x => x.AggregateId, id))
				.SetSortOrder(SortBy<EventEvenlope>.Ascending(x => x.Version))
				.Select(x => x.Event);
		}

		public Event GetLastEventFor(Guid id)
		{
			var envelope = _collection
				.Find(Query<EventEvenlope>.EQ(x => x.AggregateId, id))
				.SetSortOrder(SortBy<EventEvenlope>.Descending(x => x.Version))
				.FirstOrDefault();

			return envelope != null
				? envelope.Event
				: null;
		}

		public void StoreEvent<TAggregate>(Event @event) where TAggregate : IEventSourced
		{
			_collection.Insert(new EventEvenlope(@event));
		}

		public class EventEvenlope
		{
			public ObjectId Id { get; set; }
			public Guid AggregateId { get; set; }
			public DateTime Created { get; set; }
			public int Version { get; set; }
			public Event Event { get; set; }

			public EventEvenlope() { }

			public EventEvenlope(Event @event) 
			{
				Created = DateTime.UtcNow;
				AggregateId = @event.AggregateId;
				Version = @event.Version;
				Event = @event;
			}
		}
	}
}