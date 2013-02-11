using System;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TinyCQRS.Contracts;

namespace TinyCQRS.Infrastructure.Persistence
{
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

		public T Get<T>(BlobReference<T> reference)
		{
			var result = _data.FindOneById(Blob.CreateId(reference));

			if (result == null)
			{
				throw new ApplicationException("No blob found for reference " + reference);
			}

			return (T)result.Payload;
		}

		public T Find<T>(BlobReference<T> reference)
		{
			var result = _data.FindOneById(Blob.CreateId(reference));

			if (result == null)
				return default(T);

			return (T) result.Payload;
		}

		public void Save<T>(BlobReference<T> reference)
		{
			_data.Save(new Blob(reference, reference.Payload));
		}

		public void Remove(BlobReference reference)
		{
			_data.Remove(Query.EQ("_id", Blob.CreateId(reference)));
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

			public Blob(BlobReference reference, object payload) : this()
			{
				Id = CreateId(reference);
				Payload = payload;
			}

			public static string CreateId(BlobReference reference)
			{
				return "blob_" + reference;
			}
		}
	}
}