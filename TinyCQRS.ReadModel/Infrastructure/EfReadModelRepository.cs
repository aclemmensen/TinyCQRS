using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using TinyCQRS.Contracts;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Infrastructure
{
	//public class EfReadModelRepository<T> : IReadModelRepository<T> where T : class, IReadModel, new()
	//{
	//	private readonly DbContext _context;

	//	private IDbSet<T> Set { get { return _context.Set<T>(); } }

	//	public EfReadModelRepository(DbContext context)
	//	{
	//		_context = context;
	//	}

	//	public T Get(object id)
	//	{
	//		var result = Set.Find(id);
			
	//		if (result == null)
	//		{
	//			var msg = string.Format("No {0} with id {1} found", typeof (T).Name, id);
	//			throw new ApplicationException(msg);
	//		}

	//		return result;
	//	}

	//	public IQueryable<T> All(params Expression<Func<T,object>>[] including)
	//	{
	//		return Including(including);
	//	}

	//	public IQueryable<T> Where(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] including)
	//	{
	//		return Including(including).Where(predicate);
	//	}

	//	public void Add(T dto)
	//	{
	//		Set.Add(dto);
	//		_context.Entry(dto).State = EntityState.Added;
	//	}

	//	public void Update(T dto)
	//	{
	//		Set.Attach(dto);
	//		_context.Entry(dto).State = EntityState.Modified;
	//	}

	//	public void Delete(T dto)
	//	{
	//		Set.Remove(dto);
	//		_context.Entry(dto).State = EntityState.Deleted;
	//	}

	//	public void Commit()
	//	{
	//		_context.SaveChanges();
	//	}

	//	public T Create()
	//	{
	//		return Set.Create<T>();
	//	}

	//	protected IQueryable<T> Including(Expression<Func<T, object>>[] expressions)
	//	{
	//		return expressions == null 
	//			? Set 
	//			: expressions.Aggregate<Expression<Func<T, object>>, IQueryable<T>>(Set, (current, expr) => current.Include(expr));
	//	}
	//}
}