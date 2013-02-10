using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using TinyCQRS.WebClient.Controllers;

namespace TinyCQRS.WebClient.Util
{
	public class WebServiceInstaller : IWindsorInstaller
	{
		private readonly IWindsorInstaller _inner;

		public WebServiceInstaller(IWindsorInstaller inner)
		{
			_inner = inner;
		}

		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Classes.FromAssemblyContaining<HomeController>()
				.BasedOn<Controller>()
				.WithServiceSelf()
				.LifestylePerWebRequest());

			_inner.Install(container, store);
		}
	}
}