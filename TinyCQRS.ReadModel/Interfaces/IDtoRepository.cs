using System;
using System.Collections.Generic;

namespace TinyCQRS.ReadModel.Interfaces
{
    public interface IDtoRepository<T> where T : IDto, new()
    {
        T GetById(Guid id);
        IEnumerable<T> GetAll();
        void Save(T dto);
    }
}