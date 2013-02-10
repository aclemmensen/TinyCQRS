using System;
using System.Collections.Generic;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Models;

namespace TinyCQRS.Contracts.Services
{
	public interface IService
	{
		
	}

	public interface ISiteCrawlService : IService
	{
		void StartCrawl(StartCrawl command);

		void RegisterNewPage(RegisterNewPage command);

		void UpdatePageContent(RegisterPageContentChange command);

		void PageCheckedWithoutChanges(RegisterNoChangeCheck command);

		void MarkCrawlComplete(MarkCrawlComplete command);

		CrawlSpec GetCrawlInfoFor(Guid siteId);
	}

	public interface ISetupService : IService
	{
		void CreateNewSite(CreateNewSite command);
	}

	public interface ISiteService : IService
	{
		void OrderFullCrawl(OrderCrawl command);

		void OrderPageCheck(OrderPageCheck command);

		SiteIdentity GetSiteIdentity(Guid siteId);

		SiteOverview GetSiteOverview(Guid siteId);

		IEnumerable<SiteIdentity> GetAllSites();

		IEnumerable<Page> GetPagesFor(Guid siteId);
	}
}