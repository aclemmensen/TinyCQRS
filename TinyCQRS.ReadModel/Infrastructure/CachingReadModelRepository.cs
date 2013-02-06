using System;
using System.Collections.Generic;
using System.Linq;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Infrastructure
{
	public class CachingReadModelRepository<T> : IReadModelRepository<T> where T : Entity, new()
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
}