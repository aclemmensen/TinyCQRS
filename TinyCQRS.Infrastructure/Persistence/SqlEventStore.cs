using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TinyCQRS.Contracts;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class SqlEventStore : IEventStore
	{
		private readonly string _connstr;

		public int Processed { get { return _processed; } }
		private int _processed;

		public SqlEventStore(string connstr)
		{
			_connstr = connstr;
		}

		public IEnumerable<Event> GetEventsFor<T>(Guid id) where T : IEventSourced
		{
			return GetEventCollectionFor<T>(id);
		}

		public int GetVersionFor<T>(Guid id) where T : IEventSourced
		{
			var last = GetEventCollectionFor<T>(id).LastOrDefault();
			return last == null ? 0 : last.Version;
		}

		public void StoreEvent<TAggregate>(Event @event) where TAggregate : IEventSourced
		{
			_processed++;

			var envelope = new MessageEnvelope(@event);

			if (envelope.AggregateId == Guid.Empty)
			{
				throw new InvalidOperationException("Cannot save event without aggregate id");
			}

			var affected = Execute(
				string.Format("INSERT INTO Events (AggregateId, MessageId, CorrelationId, Version, Created, Data, Type) VALUES(@id, @mid, @cid, @v, @c, @d, @t)"),
				new Dictionary<string, object>
				{
					{"id", envelope.AggregateId},
					{"mid", envelope.MessageId},
					{"cid", envelope.CorrelationId},
					{"v", envelope.Version},
					{"c", envelope.Created},
					{"d", envelope.Data},
					{"t", envelope.Type}
				});

			if (affected != 1)
			{
				throw new ApplicationException("Event storage did not succeed");
			}

		}

		private IEnumerable<Event> GetEventCollectionFor<T>(Guid id) where T : IEventSourced
		{
			return 
				Query(
					string.Format("SELECT * FROM Events WHERE AggregateId = @id ORDER BY Version ASC"),
					EventFactory,
					new Dictionary<string, object> { { "id", id } })
					.Select(x => x.Event)
					.ToList();
		}

		private static MessageEnvelope EventFactory(IReadOnlyDictionary<string, object> data)
		{
			return new MessageEnvelope
			{
				AggregateId = Guid.Parse(data["AggregateId"].ToString()),
				CorrelationId = Guid.Parse(data["CorrelationId"].ToString()),
				Created = DateTime.Parse(data["Created"].ToString()),
				Version = int.Parse(data["Version"].ToString()),
				Data = data["Data"].ToString(),
				Type = data["Type"].ToString()
			};
		}

		private static SqlCommand CreateCommand(SqlConnection conn, string statement, Dictionary<string,object> parameters = null)
		{
			var cmd = conn.CreateCommand();
			cmd.CommandText = statement;

			if (parameters != null)
			{
				foreach (var p in parameters)
				{
					cmd.Parameters.AddWithValue("@" + p.Key, p.Value ?? DBNull.Value);
				}
			}

			conn.Open();

			return cmd;
		}

		private IEnumerable<T> Query<T>(string statement, Func<IReadOnlyDictionary<string,object>,T> factory, Dictionary<string,object> parameters = null)
		{
			using (var conn = new SqlConnection(_connstr))
			using (var cmd = CreateCommand(conn, statement, parameters))
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					var row = new Dictionary<string, object>();

					for (var i = 0; i < reader.FieldCount; i++)
					{
						row.Add(reader.GetName(i), reader.GetValue(i));
					}

					yield return factory(row);
				}
			}
		}

		private int Execute(string statement, Dictionary<string,object> parameters = null)
		{
			using (var conn = new SqlConnection(_connstr))
			using (var cmd = CreateCommand(conn, statement, parameters))
			{
				var count = cmd.ExecuteNonQuery();
				return count;
			}
		}
	}
}