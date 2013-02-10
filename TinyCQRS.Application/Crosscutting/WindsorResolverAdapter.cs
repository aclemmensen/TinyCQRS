using System;
using System.Collections.Generic;
using Castle.Windsor;
using TinyCQRS.Infrastructure;

namespace TinyCQRS.Application.Crosscutting
{
	public class WindsorResolverAdapter : IResolver
	{
		private readonly IWindsorContainer _container;

		public WindsorResolverAdapter(IWindsorContainer container)
		{
			_container = container;
		}

		public object Resolve(Type type)
		{
			return _container.Resolve(type);
		}

		public T Resolve<T>()
		{
			return _container.Resolve<T>();
		}

		public object[] ResolveAll(Type type)
		{
			return (object[])_container.ResolveAll(type);
		}

		public IEnumerable<T> ResolveAll<T>()
		{
			return _container.ResolveAll<T>();
		}
	}
}