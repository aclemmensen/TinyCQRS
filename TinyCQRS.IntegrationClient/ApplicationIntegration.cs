using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using TinyCQRS.Application.Crosscutting;
using TinyCQRS.Application.Modules.Crawler;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Services;
using TinyCQRS.Infrastructure;

namespace TinyCQRS.IntegrationClient
{
	public class ApplicationIntegration
	{
		private readonly string _name;
		private readonly string _root;
		private readonly ISiteService _siteService;
		private readonly ICrawlService _crawlService;
		private readonly ISetupService _setupService;
		private readonly Crawler _crawler;
		private Guid _crawlId;

		public ApplicationIntegration(string name, string root)
		{
			_name = name;
			_root = root;
			var container = new WindsorContainer();
			container.Install(
				new DatabaseServiceInstaller(
					@"Data Source=.\SQLExpress;Integrated Security=true;Database=TinyCQRS.Events",
					@"Data Source=.\SQLExpress;Integrated Security=true;Database=TinyCQRS.ReadModelDenormalized",
					@"mongodb://localhost"));

			container.Register(Component.For<ILogger>().ImplementedBy<ConsoleLogger>().IsDefault());

			_siteService = container.Resolve<ISiteService>();
			_setupService = container.Resolve<ISetupService>();
			_crawlService = container.Resolve<ICrawlService>();

			_crawler = new Crawler(_crawlService, container.Resolve<ILogger>());
		}

		public void CrawlExisting(Guid siteId)
		{
			_crawlId = Guid.NewGuid();

			_siteService.OrderFullCrawl(new OrderCrawl(_crawlId, siteId, DateTime.UtcNow));
			_crawler.Crawl(_crawlId, siteId);
		}

		public void CrawlNew(Guid siteId)
		{
			_setupService.CreateNewSite(new CreateNewSite(siteId, _name, _root));
			CrawlExisting(siteId);
		}

		public void Handle(string url, string content)
		{
			_crawler.Handle(url, content);
		}

		public void Done()
		{
			_crawler.Done();
		}
	}
}
