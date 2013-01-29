using System;
using System.Collections.Generic;

namespace TinyCQRS.Domain.Interfaces
{
    public interface IEventedRepository<T> where T : AggregateRoot, new()
    {
        T GetById(Guid id);
        void Save(AggregateRoot aggregate, int? expectedVersion = null);
    }
}