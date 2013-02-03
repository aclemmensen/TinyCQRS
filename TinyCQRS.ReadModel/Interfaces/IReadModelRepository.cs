using System;
using System.Collections.Generic;
using System.Linq;

namespace TinyCQRS.ReadModel.Interfaces
{
    public interface IReadModelRepository<T> where T : IDto, new()
    {
	    T Get(Guid aggregateId);

        IQueryable<T> All();

		IQueryable<T> Where(Func<T, bool> predicate);

        void Add(T dto);

	    void Update(T dto);

	    void Commit();
    }
}