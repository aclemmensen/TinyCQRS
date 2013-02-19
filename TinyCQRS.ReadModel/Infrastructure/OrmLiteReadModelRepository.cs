using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using ServiceStack.OrmLite;
using TinyCQRS.Contracts;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Infrastructure
{
	public class OrmLiteReadModelRepository<T> : IReadModelRepository<T> where T : Dto, new()
	{
		private readonly string _connstr;
		private readonly OrmLiteConnectionFactory _connfac;
		//private IDbTransaction _transaction;

		private IDbConnection Connection { get { return _connfac.OpenDbConnection(); } }

		public OrmLiteReadModelRepository(string connstr)
		{
			_connstr = connstr;
			_connfac = new OrmLiteConnectionFactory(_connstr, true, SqlServerDialect.Provider)
			{
				//AlwaysReturnTransaction = _transaction
			};
			
			//_transaction = Connection.BeginTransaction();

			Connection.CreateTableIfNotExists<T>();
		}

		public T Find(object id)
		{
			throw new NotImplementedException();
		}

		public T Get(object id)
		{
			return Connection.Id<T>(id);
		}

		public IQueryable<T> All(params Expression<Func<T, object>>[] including)
		{
			return Connection.Each<T>().AsQueryable();
		}

		public IQueryable<T> Where(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] including)
		{
			return Connection.Select(predicate).AsQueryable();
		}

		public void Add(T dto)
		{
			Connection.Insert(dto);
		}

		public void Update(T dto)
		{
			Connection.Update(dto);
		}

		public void Delete(T dto)
		{
			Connection.Delete(dto);
		}

		public void Commit()
		{
			//_transaction.Commit();
			//_transaction = Connection.BeginTransaction();
		}

		public T Create()
		{
			return new T();
		}
	}
}