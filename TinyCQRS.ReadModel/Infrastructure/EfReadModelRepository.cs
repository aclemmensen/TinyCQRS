using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Infrastructure
{
	public class CachingReadModelRepository<T> : IReadModelRepository<T> where T : Dto, new()
	{
		private readonly IReadModelRepository<T> _innerRepository;
		private readonly Dictionary<Guid,T> _cache = new Dictionary<Guid, T>();

		public CachingReadModelRepository(IReadModelRepository<T> innerRepository)
		{
			_innerRepository = innerRepository;
		}

		public T Get(Guid aggregateId)
		{
			if (!_cache.ContainsKey(aggregateId))
			{
				_cache[aggregateId] = _innerRepository.Get(aggregateId);
			}

			return _cache[aggregateId];
		}

		public IQueryable<T> All()
		{
			return _innerRepository.All();
		}

		public IQueryable<T> Where(Func<T, bool> predicate)
		{
			return _innerRepository.Where(predicate);
		}

		public void Add(T dto)
		{
			_cache[dto.Id] = dto;
			_innerRepository.Add(dto);
		}

		public void Update(T dto)
		{
			_innerRepository.Update(dto);
		}

		public void Commit()
		{
			_innerRepository.Commit();
		}

		public T Create()
		{
			return _innerRepository.Create();
		}
	}

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