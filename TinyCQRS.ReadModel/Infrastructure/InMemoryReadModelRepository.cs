using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TinyCQRS.Contracts;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Infrastructure
{
    public class InMemoryReadModelRepository<T> : IReadModelRepository<T> where T : Entity, new()
    {
        private readonly Dictionary<object,T> _data = new Dictionary<object, T>();

		public IQueryable<T> Where(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] including)
	    {
		    return _data.Values.Where(predicate.Compile()).AsQueryable();
	    }

	    public void Add(T dto)
        {
            _data[dto.Id ?? dto.GlobalId] = dto;
        }

	    public void Update(T dto)
	    {
		    _data.Remove(dto.Id ?? dto.GlobalId);
		    _data[dto.Id ?? dto.GlobalId] = dto;
	    }

	    public void Delete(T dto)
	    {
		    _data.Remove(dto.Id ?? dto.GlobalId);
	    }

	    public void Commit()
	    {
		    // NOOP
	    }

	    public T Create()
	    {
		    return new T();
	    }

		public T Find(object id)
		{
			T dto;
			if (_data.TryGetValue(id, out dto))
			{
				return dto;
			}

			return null;
		}

	    public T Get(object id)
	    {
		    var result = Find(id);

			if (result == null)
			{
				throw new ApplicationException(string.Format("No DTO of type {0} with id {1} exists", typeof(T).Name, id));
			}

		    return result;
	    }

		public IQueryable<T> All(params Expression<Func<T, object>>[] including)
        {
            return _data.Values.AsQueryable();
        }
    }
}