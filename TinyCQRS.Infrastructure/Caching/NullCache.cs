using System;

namespace TinyCQRS.Infrastructure.Caching
{
	public class NullCache<T> : ICache<T>
	{
		public T Get(Guid id, Func<T> action)
		{
			return action();
		}

		public void Set(Guid id, T item)
		{
			
		}
	}
}