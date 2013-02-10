using System;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using TinyCQRS.Contracts;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Infrastructure
{
	public class MongoReadModelRepository<T> : IReadModelRepository<T> where T : class, IReadModel, new()
	{
		private readonly MongoCollection<T> _collection;

		public MongoReadModelRepository(string connstr)
		{
			var client = new MongoClient(connstr);
			var server = client.GetServer();
			var db = server.GetDatabase("local");
			
			_collection = db.GetCollection<T>(typeof(T).Name);
		}

		public T Get(object id)
		{
			IMongoQuery q;
			
			if (id is Guid)
			{
				q = Query.EQ(KeyName(), new BsonBinaryData((Guid)id));
			}
			else
			{
				q = Query.EQ(KeyName(), id.ToString());
			}
			
			return _collection.FindOne(q);
		}

		public IQueryable<T> All(params Expression<Func<T, object>>[] including)
		{
			return _collection.AsQueryable();
		}

		public IQueryable<T> Where(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] including)
		{
			return _collection.AsQueryable().Where(predicate);
		}

		public void Add(T dto)
		{
			dto.Id = dto.Id();
			

			_collection.Insert(dto);
		}

		public void Update(T dto)
		{
			_collection.Save(dto);
		}

		public void Delete(T dto)
		{
			_collection.Remove(Query.EQ(KeyName(), dto.Id.ToString()));
		}

		public void Commit()
		{
			
		}

		public T Create()
		{
			return new T();
		}

		private static string KeyName()
		{
			if (typeof(Entity).IsAssignableFrom(typeof(T)))
			{
				return "GlobalId";
			}

			return typeof(Dto).IsAssignableFrom(typeof(T)) 
				? "LocalId" 
				: "_id";
		}
	}
}