﻿using System;
using System.Linq;
using System.Linq.Expressions;
using TinyCQRS.Contracts;

namespace TinyCQRS.ReadModel.Interfaces
{
	public interface IReadModelRepository<T> where T : Dto, new()
    {
	    T Find(object id);

	    T Get(object id);

	    IQueryable<T> All(params Expression<Func<T, object>>[] including);

	    IQueryable<T> Where(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] including);

        void Add(T dto);

	    void Update(T dto);

	    void Delete(T dto);

	    void Commit();

	    T Create();
    }
}