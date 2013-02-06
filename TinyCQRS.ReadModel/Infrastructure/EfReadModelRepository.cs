using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Infrastructure
{
	public class EfReadModelRepository<T> : IReadModelRepository<T> where T : Entity, new()
	{
		private readonly DbContext _context;

		private IDbSet<T> Set { get { return _context.Set<T>(); } }

		public EfReadModelRepository(DbContext context)
		{
			_context = context;
		}

		public T Get(Guid id)
		{
			var result = Set.Find(id);

			if (result == null)
			{
				var msg = string.Format("No {0} with id {1} found", typeof (T).Name, id);
				throw new ApplicationException(msg);
			}

			return result;
		}

		public IQueryable<T> All()
		{
			return Set;
		}

		public IQueryable<T> Where(Func<T, bool> predicate)
		{
			return Set.Where(predicate).AsQueryable();
		}

		public void Add(T dto)
		{
			Guard(dto);
			Set.Add(dto);
			_context.Entry(dto).State = EntityState.Added;
		}

		public void Update(T dto)
		{
			Guard(dto);
			Set.Attach(dto);
			_context.Entry(dto).State = EntityState.Modified;
		}

		public void Commit()
		{
			_context.SaveChanges();
		}

		public T Create()
		{
			return Set.Create<T>();
		}

		private void Guard(T dto)
		{
			if (dto.Id == Guid.Empty)
			{
				throw new InvalidDataException("Cannot store entity with empty key.");
			}
		}
	}
}