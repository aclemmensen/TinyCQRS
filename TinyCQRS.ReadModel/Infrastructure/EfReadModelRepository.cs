using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Infrastructure
{
	public class EfReadModelRepository<T> : IReadModelRepository<T> where T : Dto, new()
	{
		private readonly DbContext _context;

		private IDbSet<T> Set { get { return _context.Set<T>(); } }

		public EfReadModelRepository(DbContext context)
		{
			_context = context;
		}

		public T Get(Guid id)
		{
			return Set.Find(id);
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

		private void Guard(T dto)
		{
			if (dto.Id == Guid.Empty)
			{
				throw new InvalidDataException("Cannot store entity with empty key.");
			}
		}
	}
}