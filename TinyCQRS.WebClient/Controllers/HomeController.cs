using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TinyCQRS.Application.Modules.Crawler;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Services;
using TinyCQRS.WebClient.Models;

namespace TinyCQRS.WebClient.Controllers
{
	public class HomeController : Controller
	{
		private readonly ISiteService _siteService;
		private readonly ISetupService _setup;

		public HomeController(ISiteService siteService, ISetupService setup)
		{
			_siteService = siteService;
			_setup = setup;
		}

		public ActionResult Index()
		{
			var vm = new SiteIndexViewModel()
			{
				Sites = _siteService.GetAllSites()
			};

			return View(vm);
		}

		public ActionResult CreateSite()
		{
			_setup.CreateNewSite(new CreateNewSite(Guid.NewGuid(), "From web app", "http://garbage.dk"));
			return RedirectToAction("Index");
		}

		public ActionResult Overview(Guid id)
		{
			var vm = new SiteOverviewViewModel
			{
				Site = _siteService.GetSiteOverview(id)
			};

			return View(vm);
		}

		public ActionResult Pages(Guid id)
		{
			var vm = new SitePagesViewModel
			{
				Site = _siteService.GetSiteIdentity(id),
				Pages = _siteService.GetPagesFor(id)
			};

			return View(vm);
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your app description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}

	}
}
