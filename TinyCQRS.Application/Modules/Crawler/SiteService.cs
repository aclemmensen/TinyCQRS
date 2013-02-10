using System;
using System.Collections.Generic;
using System.Linq;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Models;
using TinyCQRS.Contracts.Services;
using TinyCQRS.Infrastructure;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.Application.Modules.Crawler
{
	public class SiteService : ISiteService
	{
		private readonly ICommandDispatcher _dispatcher;
		private readonly IReadModelRepository<Site> _sites;
		private readonly IReadModelRepository<Page> _pages;
		private readonly IReadModelRepository<SiteIdentity> _identities;
		private readonly IReadModelRepository<SiteOverview> _overviews;

		public SiteService(
			ICommandDispatcher dispatcher, 
			IReadModelRepository<Site> sites, 
			IReadModelRepository<Page> pages, 
			IReadModelRepository<SiteIdentity> identities,
			IReadModelRepository<SiteOverview> overviews)
		{
			_dispatcher = dispatcher;
			_sites = sites;
			_pages = pages;
			_identities = identities;
			_overviews = overviews;
		}

		public void OrderFullCrawl(OrderCrawl command)
		{
			_dispatcher.Dispatch(command);
		}

		public void OrderPageCheck(OrderPageCheck command)
		{
			_dispatcher.Dispatch(command);
		}

		public SiteIdentity GetSiteIdentity(Guid siteId)
		{
			//return _sites.Where(x => x.GlobalId == siteId, x => x.Crawls.Select(y => y.Checks)).SingleOrDefault();
			return _identities.Get(siteId);
		}

		public SiteOverview GetSiteOverview(Guid siteId)
		{
			return _overviews.Get(siteId);
		}

		public IEnumerable<SiteIdentity> GetAllSites()
		{
			return _identities.All();
		}

		public IEnumerable<Page> GetPagesFor(Guid siteId)
		{
			//return _pages.Where(x => x.SiteId == siteId, x => x.Checks).OrderBy(x => x.FirstSeen).ToList();
			return _sites.Get(siteId).Pages.OrderBy(x => x.FirstSeen);
		}
	}
}