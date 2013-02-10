using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TinyCQRS.Contracts;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class SqlEventStore<T> : IEventStore<T> where T : EventSourced
	{
		private readonly string _connstr;

		public int Processed { get { return _processed; } }
		private int _processed;
		private string _location;

		public SqlEventStore(string connstr)
		{
			_connstr = connstr;
			_location = typeof (AggregateRoot).IsAssignableFrom(typeof(T)) ? "Aggregates" : "Sagas";
		}

		public IEnumerable<Event> GetEventsFor(Guid id)
		{
			return GetEventCollectionFor(id);
		}

		public Event GetLastEventFor(Guid id)
		{
			return GetEventCollectionFor(id).LastOrDefault();
		}

		public void StoreEvent(Event @event)
		{
			_processed++;

			var envelope = new MessageEnvelope(@event);

			if (envelope.AggregateId == Guid.Empty)
			{
				throw new InvalidOperationException("Cannot save event without aggregate id");
			}

			var affected = Execute(
				string.Format("INSERT INTO {0} (AggregateId, MessageId, CorrelationId, Version, Created, Data, Type) VALUES(@id, @mid, @cid, @v, @c, @d, @t)", _location),
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

		private IList<Event> GetEventCollectionFor(Guid id)
		{
			return 
				Query(
					string.Format("SELECT * FROM {0} WHERE AggregateId = @id ORDER BY Version ASC", _location),
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
				var count = 0;

				while (reader.Read())
				{
					var row = new Dictionary<string, object>();
					count++;

					for (var i = 0; i < reader.FieldCount; i++)
					{
						row.Add(reader.GetName(i), reader.GetValue(i));
					}

					yield return factory(row);
				}

				Console.WriteLine("Fetched {0} rows", count);
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