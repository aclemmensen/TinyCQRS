using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using ServiceStack.Text;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using TinyCQRS.Contracts;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class OrmLiteEventStore : IEventStore
	{
		private readonly OrmLiteConnectionFactory _connfac;
		private const string GetLastEvent = @"select e.* from Events e join Aggregates a on e.AggregateId = a.AggregateId and e.Version = a.Version where a.AggregateId = @id";

		public int Processed { get; private set; }

		public OrmLiteEventStore(string connstr)
		{
			_connfac = new OrmLiteConnectionFactory(connstr, true, new SqlServerOrmLiteDialectProvider())
			{
				ConnectionFilter = x => new ProfiledDbConnection(x as DbConnection, MiniProfiler.Current)
			};
			
			using (var conn = _connfac.OpenDbConnection())
			{
				conn.CreateTableIfNotExists<EventEnvelope>();
				conn.CreateTableIfNotExists<AggregateStatus>();
			}
		}

		public IEnumerable<Event> GetEventsFor(Guid id)
		{
			using (var conn = _connfac.OpenDbConnection())
			{
				return conn.Select<EventEnvelope>(x => x.AggregateId == id).OrderBy(x => x.Version).Select(x => x.Event);
			}
		}

		public Event GetLastEventFor(Guid id)
		{
			using (var conn = _connfac.OpenDbConnection())
			{
				var result = conn.QuerySingle<EventEnvelope>(GetLastEvent, new { id });
				return result == null ? null : result.Event;
			}
		}

		public void StoreEvent<TAggregate>(Event @event) where TAggregate : IEventSourced
		{
			using (var conn = _connfac.OpenDbConnection())
			using (var tx = conn.OpenTransaction())
			{
				conn.Insert(new EventEnvelope(@event));

				if (@event.Version > 1)
				{
					conn.Update<AggregateStatus>(new { @event.Version }, s => s.AggregateId == @event.AggregateId);
				}
				else
				{
					conn.Insert(AggregateStatus.Create(@event.AggregateId, @event.Version, typeof(TAggregate).Name));
				}

				tx.Commit();
			}
		}

		[Alias("Events")]
		public class EventEnvelope
		{
			[AutoIncrement, PrimaryKey]
			public int Id { get; set; }

			[Index]
			public Guid AggregateId { get; set; }

			[Index(true)]
			public Guid MessageId { get; set; }

			[Index]
			public Guid CorrelationId { get; set; }

			public int Version { get; set; }
			public DateTime Created { get; set; }

			public string Payload { get; set; }
			public string EventType { get; set; }
			public Type Type { get; set; }

			private Event _cache;

			[Ignore]
			public Event Event
			{
				get { return _cache ?? (_cache = (Event)TypeSerializer.DeserializeFromString(Payload, Type)); }
				set
				{
					var x = value;
					Payload = TypeSerializer.SerializeToString(x);
					_cache = x;
				}
			}

			public EventEnvelope() { }

			public EventEnvelope(Event @event)
			{
				AggregateId = @event.AggregateId;
				MessageId = Guid.NewGuid();
				CorrelationId = @event.CorrelationId;
				Version = @event.Version;
				Created = DateTime.Now;
				Event = @event;
				Type = @event.GetType();
				EventType = Type.Name;
			}
		}

		[Alias("Aggregates")]
		public class AggregateStatus
		{
			[AutoIncrement, PrimaryKey]
			public int Id { get; set; }
			[Index(true)]
			public Guid AggregateId { get; set; }
			public int Version { get; set; }
			public string Type { get; set; }

			public AggregateStatus() { }

			public static AggregateStatus Create(Guid aggregateId, int version, string type)
			{
				return new AggregateStatus
				{
					AggregateId = aggregateId,
					Version = version,
					Type = type
				};
			}
		}
	}

}