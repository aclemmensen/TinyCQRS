using System;
using System.Collections.Generic;

namespace TinyCQRS.Infrastructure
{
	public interface IResolver
	{
		IRelease<object> Resolve(Type type);
		IRelease<T> Resolve<T>();

		IRelease<object[]> ResolveAll(Type type);
		IRelease<IEnumerable<T>> ResolveAll<T>();
	}

	public interface IRelease<T> : IDisposable
	{
		T Instance { get; }
	}
}