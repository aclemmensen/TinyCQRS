using System;

namespace TinyCQRS.Infrastructure.Caching
{
	public interface ICache<T>
	{
		T Get(Guid id, Func<T> action);
		void Set(Guid id, T item);
	}
}