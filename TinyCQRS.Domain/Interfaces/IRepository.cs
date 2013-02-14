using System;
using System.Collections.Generic;

namespace TinyCQRS.Domain.Interfaces
{
    public interface IRepository<T> where T : IEventSourced, new()
    {
        T GetById(Guid id);
        void Save(T aggregate, int? expectedVersion = null);
    }
}