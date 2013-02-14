using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TinyCQRS.Contracts;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class MemoryBlobStorage : IBlobStorage
	{
		private readonly Dictionary<Guid,Dictionary<Guid,object>> _data = new Dictionary<Guid, Dictionary<Guid, object>>();
		
		public T Get<T>(BlobReference reference)
		{
			return (T) _data[reference.AggregateId][reference.ItemId];
		}

		public T Find<T>(BlobReference reference)
		{
			return Get<T>(reference);
		}

		public void Save<T>(BlobReference reference, T payload)
		{
			if (!_data.ContainsKey(reference.AggregateId))
			{
				_data[reference.AggregateId] = new Dictionary<Guid, object>();
			}

			_data[reference.AggregateId][reference.ItemId] = payload;
		}

		public void Remove(BlobReference reference)
		{
			throw new NotImplementedException();
		}
	}

	public class MongoBlobStorage : IBlobStorage
	{
		private readonly MongoDatabase _db;
		private readonly MongoCollection<Blob> _data;

		public MongoBlobStorage(string connstr)
		{
			var client = new MongoClient(connstr);
			var server = client.GetServer();
			_db = server.GetDatabase("storage");
			_data = _db.GetCollection<Blob>("Blobs");
		}

		public T Get<T>(BlobReference reference)
		{
			var result = _data.FindOneById(reference.ToString());

			if (result == null)
			{
				throw new ApplicationException("No blob found for reference " + reference.ToString());
			}

			return (T)result.Payload;
		}

		public T Find<T>(BlobReference reference)
		{
			var result = _data.FindOneById(reference.ToString());

			if (result == null)
				return default(T);

			return (T)result.Payload;
		}

		public void Save<T>(BlobReference reference, T payload)
		{
			_data.Save(Blob.Create<T>(reference, payload));
		}

		public void Remove(BlobReference reference)
		{
			_data.Remove(Query.EQ("_id", reference.ToString()));
		}

		private class Blob
		{
			public string Id { get; set; }
			public object Payload { get; set; }
			public DateTime Created { get; set; }

			private Blob()
			{
				Created = DateTime.UtcNow;
			}

			public static Blob Create<T>(BlobReference reference, object payload = null)
			{
				return new Blob
				{
					Id = reference.ToString(),
					Payload = payload
				};
			}
		}
	}
}