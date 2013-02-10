using System;
using System.Collections.Generic;

namespace TinyCQRS.Infrastructure
{
	public interface IResolver
	{
		object Resolve(Type type);
		T Resolve<T>();

		object[] ResolveAll(Type type);
		IEnumerable<T> ResolveAll<T>();
	}
}