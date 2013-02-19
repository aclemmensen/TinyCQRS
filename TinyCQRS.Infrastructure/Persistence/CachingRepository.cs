using System;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Infrastructure.Caching;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class CachingRepository<T> : IRepository<T> where T : IEventSourced, new()
	{
		private readonly IRepository<T> _innerRepository;
		private readonly ICache<T> _cache;

		public CachingRepository(IRepository<T> innerRepository, ICache<T> cache)
		{
			_innerRepository = innerRepository;
			_cache = cache;
		}

		public T GetById(Guid id)
		{
			return _cache.Get(id, () => _innerRepository.GetById(id));
		}

		public void Save(T aggregate, int? expectedVersion = null)
		{
			_innerRepository.Save(aggregate, expectedVersion);

			_cache.Set(aggregate.Id, aggregate);
		}
	}
}