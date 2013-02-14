using System;
using System.Collections.Generic;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Models;

namespace TinyCQRS.Contracts.Services
{
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