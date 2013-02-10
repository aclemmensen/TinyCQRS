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

		public IRelease<object> Resolve(Type type)
		{
			return new WindsorRelease<object>(_container, _container.Resolve(type));
		}

		public IRelease<T> Resolve<T>()
		{
			return new WindsorRelease<T>(_container, _container.Resolve<T>());
		}

		public IRelease<object[]> ResolveAll(Type type)
		{
			return new WindsorRelease<object[]>(_container, (object[]) _container.ResolveAll(type));
		}

		public IRelease<IEnumerable<T>> ResolveAll<T>()
		{
			var instances = _container.ResolveAll<T>();
			return new WindsorRelease<IEnumerable<T>>(_container, instances);
		}
	}

	public class WindsorRelease<T> : IRelease<T>
	{
		private readonly IWindsorContainer _container;

		public WindsorRelease(IWindsorContainer container, T instance)
		{
			_container = container;
			Instance = instance;
		}

		public void Dispose()
		{
			_container.Release(Instance);
		}

		public T Instance { get; private set; }
	}
}