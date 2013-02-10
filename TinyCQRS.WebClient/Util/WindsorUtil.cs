using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;

namespace TinyCQRS.WebClient.Util
{
	public class WindsorControllerFactory : DefaultControllerFactory
	{
		private readonly IWindsorContainer _container;

		public WindsorControllerFactory(IWindsorContainer container)
		{
			_container = container;
		}

		protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
		{
			if (controllerType == null)
			{
				throw new Exception("No controller type provided");
			}

			return (IController)_container.Resolve(controllerType);
		}

		public override void ReleaseController(IController controller)
		{
			_container.Release(controller);
		}
	}

	public class WindsorRoot : IHttpControllerActivator
	{
		private readonly IWindsorContainer _container;

		public WindsorRoot(IWindsorContainer container)
		{
			_container = container;
		}

		public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
		{
			var controller = (IHttpController)_container.Resolve(controllerType);

			request.RegisterForDispose(new Release(() => _container.Release(controller)));

			return controller;
		}

		private class Release : IDisposable
		{
			private readonly Action _release;

			public Release(Action action)
			{
				_release = action;
			}

			public void Dispose()
			{
				_release();
			}
		}
	}
}