﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class SqlEventStore : IEventStore
	{
		private readonly string _connstr;
		private Dictionary<Guid,List<Event>> _cache = new Dictionary<Guid, List<Event>>();

		public SqlEventStore(string connstr)
		{
			_connstr = connstr;
		}

		public IEnumerable<Event> GetEventsFor(Guid id)
		{
			if (!_cache.ContainsKey(id))
			{
				var result = Query(
					"SELECT * FROM EventEnvelopes WHERE AggregateId = @id ORDER BY Version ASC",
					EventFactory,
					new Dictionary<string, object> { { "id", id } });

				_cache.Add(id, new List<Event>(result.Select(x => x.Event)));
			}

			return _cache[id];
		}

		public Event GetLastEventFor(Guid id)
		{
			if (!_cache.ContainsKey(id))
			{
				GetEventsFor(id);
			}
			
			return _cache[id].LastOrDefault();
		}

		public void StoreEvent(Event @event)
		{
			var envelope = new EventEnvelope(@event);

			if (envelope.AggregateId == Guid.Empty)
			{
				throw new InvalidOperationException("Cannot save event without aggregate id");
			}

			var affected = Execute(
				"INSERT INTO EventEnvelopes (AggregateId, CorrelationId, Version, Created, Data, Type) VALUES(@id, @cid, @v, @c, @d, @t)",
				new Dictionary<string, object>
				{
					{"id", envelope.AggregateId},
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

			if (_cache.ContainsKey(@event.AggregateId))
			{
				_cache[@event.AggregateId].Add(@event);
			}
		}

		private static EventEnvelope EventFactory(IReadOnlyDictionary<string, object> data)
		{
			return new EventEnvelope
			{
				AggregateId = Guid.Parse(data["AggregateId"].ToString()),
				CorrelationId = Guid.Parse(data["CorrelationId"].ToString()),
				Created = DateTime.Parse(data["Created"].ToString()),
				Version = int.Parse(data["Version"].ToString()),
				Data = data["Data"].ToString(),
				Type = data["Type"].ToString()
			};
		}

		private SqlCommand CreateCommand(SqlConnection conn, string statement, Dictionary<string,object> parameters = null)
		{
			var cmd = conn.CreateCommand();
			cmd.CommandText = statement;

			if (parameters != null)
			{
				foreach (var p in parameters)
				{
					cmd.Parameters.AddWithValue("@" + p.Key, p.Value);
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
				return cmd.ExecuteNonQuery();
			}
		}
	}
}