using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using StackExchange.Profiling;
using TinyCQRS.Application.Crosscutting;
using TinyCQRS.WebClient.Util;

namespace TinyCQRS.WebClient
{
	public class MvcApplication : System.Web.HttpApplication
	{
		private IWindsorContainer _container;

		public MvcApplication()
		{
			_container = new WindsorContainer();
		}

		protected void Application_Start()
		{
			_container.Install(new WebServiceInstaller(new DatabaseServiceInstaller(
				@"Data Source=.\SQLExpress;Integrated Security=true;Database=TinyCQRS.Events",
				@"Data Source=.\SQLExpress;Integrated Security=true;Database=TinyCQRS.ReadModelDenormalized",
				@"mongodb://localhost")));

			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AuthConfig.RegisterAuth();

			GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorRoot(_container));
			ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(_container));

			MiniProfiler.Settings.SqlFormatter = new StackExchange.Profiling.SqlFormatters.SqlServerFormatter();
			MiniProfilerEF.Initialize();
		}

		protected void Application_BeginRequest()
		{
			if (Request.IsLocal)
			{
				MiniProfiler.Start();
			}
		}

		protected void Application_EndRequest()
		{
			MiniProfiler.Stop();
			_container.Dispose();
		}

		public override void Dispose()
		{
			_container.Dispose();
		}
	}
}