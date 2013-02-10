using System;
using System.Web.Mvc;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Services;

namespace TinyCQRS.WebClient.Controllers
{
	public class AdminController : Controller
	{
		private readonly IProjectionService _projections;

		public AdminController(IProjectionService projections)
		{
			_projections = projections;
		}

		public ActionResult Regenerate(Guid id)
		{
			_projections.RegenerateForSite(new RegenerateForSite(id));

			return RedirectToAction("Index", "Home");
		}
	}
}