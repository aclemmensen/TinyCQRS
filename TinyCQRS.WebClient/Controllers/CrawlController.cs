using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Services;

namespace TinyCQRS.WebClient.Controllers
{
	public class CrawlController : Controller
	{
		private readonly ISiteService _sites;
		private readonly ICrawlService _crawls;

		public CrawlController(ISiteService sites, ICrawlService crawls)
		{
			_sites = sites;
			_crawls = crawls;
		}

		public ActionResult OrderCrawlFor(Guid id)
		{
			_sites.OrderFullCrawl(new OrderCrawl(Guid.NewGuid(), id, DateTime.UtcNow));

			return RedirectToAction("Index", "Home");
		}
	}
}