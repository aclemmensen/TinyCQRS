using System;
using System.Collections.Generic;

namespace TinyCQRS.Domain.Interfaces
{
    public interface IRepository<T> where T : AggregateRoot, new()
    {
        T GetById(Guid id);
        void Save(T aggregate, int? expectedVersion = null);
    }
}