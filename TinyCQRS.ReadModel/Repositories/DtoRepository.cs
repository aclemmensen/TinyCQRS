using System;
using System.Collections.Generic;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Repositories
{
    public class DtoRepository<T> : IDtoRepository<T> where T : IDto, new()
    {
        private readonly Dictionary<Guid,T> _data = new Dictionary<Guid, T>();

        public void Save(T dto)
        {
            _data[dto.Id] = dto;
        }

        public T GetById(Guid id)
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

        public IEnumerable<T> GetAll()
        {
            return _data.Values;
        }
    }
}