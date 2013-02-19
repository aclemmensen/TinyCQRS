using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TinyCQRS.Infrastructure.Caching
{
	public class MemoryCache<T> : ICache<T>
	{
		private readonly ConcurrentDictionary<Guid, T> _data = new ConcurrentDictionary<Guid, T>();

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
				T removed;
				if (!_data.TryRemove(id, out removed))
				{
					Console.WriteLine("Could not remove {0} with id {1}", typeof(T).Name, id);
				}
			}

			_data[id] = item;
		}
	}
}