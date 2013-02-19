using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TinyCQRS.Contracts;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Infrastructure
{
	public class CachingReadModelRepository<T> : IReadModelRepository<T> where T : Dto, new()
	{
		private readonly IReadModelRepository<T> _innerRepository;
		private readonly Dictionary<object,T> _cache = new Dictionary<object, T>();

		public CachingReadModelRepository(IReadModelRepository<T> innerRepository)
		{
			_innerRepository = innerRepository;
		}

		public T Find(object id)
		{
			T obj;
			if (!_cache.TryGetValue(id, out obj) || _cache[id] == null)
			{
				_cache[id] = _innerRepository.Find(id);
			}

			return _cache[id];
		}

		public T Get(object id)
		{
			var result = Find(id);

			if (result == null)
			{
				throw new ApplicationException(string.Format("No {0} with id {1} was found", typeof(T).Name, id));
			}

			return _cache[id];
		}

		public IQueryable<T> All(params Expression<Func<T, object>>[] including)
		{
			return _innerRepository.All(including);
		}

		public IQueryable<T> Where(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] including)
		{
			return _innerRepository.Where(predicate, including);
		}

		public void Add(T dto)
		{
			_cache[dto] = dto;
			_innerRepository.Add(dto);
		}

		public void Update(T dto)
		{
			_innerRepository.Update(dto);
		}

		public void Delete(T dto)
		{
			_cache.Remove(dto.Id);
			_innerRepository.Delete(dto);
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