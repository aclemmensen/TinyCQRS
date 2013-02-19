using System;
using System.Collections.Generic;
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
	public class MongoReadModelRepository<T> : IReadModelRepository<T> where T : Dto, new()
	{
		private readonly MongoCollection<T> _collection;

		public MongoReadModelRepository(string connstr)
		{
			var client = new MongoClient(connstr);
			var server = client.GetServer();
			var db = server.GetDatabase("local");
			//db.SetProfilingLevel(ProfilingLevel.All);
			
			_collection = db.GetCollection<T>(typeof(T).Name);
		}

		public T Find(object id)
		{
			var q = Query<T>.EQ(x => x.Id, id);
			var r = _collection.FindOne(q);

			return r;
		}

		public T Get(object id)
		{
			var result = Find(id);
			
			if (result == null)
			{
				throw new ApplicationException(string.Format("Found no {0} with id {1}", typeof(T).Name, id));
			}

			return result;
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
			_collection.Insert(dto);
		}

		public void Update(T dto)
		{
			_collection.Save(dto);
		}

		public void Delete(T dto)
		{
			 _collection.Remove(Query.EQ("Id", dto.Id.ToString()));
		}

		public void Commit()
		{
			
		}

		public T Create()
		{
			return new T();
		}
	}
}