using System;
using System.Collections.Generic;
using System.Linq;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Infrastructure
{
    public class InMemoryReadModelRepository<T> : IReadModelRepository<T> where T : IDto, new()
    {
        private readonly Dictionary<Guid,T> _data = new Dictionary<Guid, T>();

	    public IQueryable<T> Where(Func<T, bool> predicate)
	    {
		    return _data.Values.Where(predicate).AsQueryable();
	    }

	    public void Add(T dto)
        {
            _data[dto.Id] = dto;
        }

	    public void Update(T dto)
	    {
		    _data.Remove(dto.Id);
		    _data[dto.Id] = dto;
	    }

	    public void Commit()
	    {
		    // NOOP
	    }

	    public T Get(Guid id)
        {
            T dto;
            if (_data.TryGetValue(id, out dto))
            {
                return dto;
            }

            throw new ApplicationException(string.Format(
                "No DTO of type {0} with id {1} exists", 
                typeof(T).Name, 
                id.ToString()));
        }

		public IQueryable<T> All()
        {
            return _data.Values.AsQueryable();
        }
    }
}