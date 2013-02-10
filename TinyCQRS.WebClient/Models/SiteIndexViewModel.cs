using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TinyCQRS.Contracts.Models;

namespace TinyCQRS.WebClient.Models
{
	public class SiteIndexViewModel
	{
		public IEnumerable<SiteIdentity> Sites { get; set; }
	}

	public class SitePagesViewModel
	{
		public SiteIdentity Site { get; set; }
		public IEnumerable<Page> Pages { get; set; }
	}

	public class SiteOverviewViewModel
	{
		public SiteOverview Site { get; set; }
	}
}