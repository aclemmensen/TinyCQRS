using System;
using System.Collections.Generic;

namespace TinyCQRS.Infrastructure.Caching
{
	public class MemoryCache<T> : ICache<T>
	{
		private readonly Dictionary<Guid,T> _data = new Dictionary<Guid, T>();

		public T Get(Guid id, Func<T> action)
		{
			if (!_data.ContainsKey(id))
			{
				_data[id] = action();
			}

			return _data[id];
		}

		public void Set(Guid id, T item)
		{
			if (_data.ContainsKey(id))
			{
				_data.Remove(id);
			}

			_data[id] = item;
		}
	}
}