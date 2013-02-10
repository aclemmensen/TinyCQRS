using System.Linq;
using TinyCQRS.Contracts;
using TinyCQRS.Contracts.Events;
using TinyCQRS.Contracts.Models;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Generators
{
	public class SiteOverviewReadModelGenerator :
		IConsume<SiteCreatedEvent>,
		IConsume<CrawlOrdered>,
		IConsume<CrawlCompleted>
	{
		private readonly IReadModelRepository<SiteOverview> _sites;

		public SiteOverviewReadModelGenerator(IReadModelRepository<SiteOverview> sites)
		{
			_sites = sites;
		}

		public void Process(CrawlCompleted @event)
		{
			_sites.CreateOrUpdate(@event.SiteId, site =>
			{
				//var brokenLinks = r.Next(0, 15);
				//var misspellings = r.Next(0, 15);

				//site.BrokenLinksCount = new ErrorCount(r.Next(brokenLinks, 25), brokenLinks, r.Next(5));
				//site.MisspellingsCount = new ErrorCount(r.Next(misspellings, 25), misspellings, r.Next(5));

				//site.History.Add(new CrawlStatusItem
				//{
				//	BrokenLinks = brokenLinks,
				//	Misspellings = misspellings,
				//	Time = @event.TimeOfCompletion
				//});

				site.PageCount = @event.PagesProcessed;
				site.LastCrawl = @event.TimeOfCompletion;
			});
		}

		public void Process(CrawlOrdered @event)
		{
			_sites.CreateOrUpdate(@event.SiteId, site =>
			{
				site.NextCrawl = @event.TimeOfOrder;
			});
		}

		public void Process(SiteCreatedEvent @event)
		{
			_sites.CreateOrUpdate(@event.AggregateId, overview =>
			{
				overview.BrokenLinksCount = new ErrorCount();
				overview.MisspellingsCount = new ErrorCount();
				overview.PageCount = 0;
			});
		}
	}

	public class SiteInventoryReadModelGenerator :
		IConsume<SiteCreatedEvent>,
		IConsume<ExistingPagesRemoved>,
		IConsume<NewPagesAdded>
	{
		private readonly IReadModelRepository<SiteInventory> _inventory;

		public SiteInventoryReadModelGenerator(IReadModelRepository<SiteInventory> inventory)
		{
			_inventory = inventory;
		}

		public void Process(SiteCreatedEvent @event)
		{
			_inventory.CreateOrUpdate(@event.AggregateId);
		}


		public void Process(ExistingPagesRemoved @event)
		{
			_inventory.CreateOrUpdate(@event.AggregateId, x =>
			{
				var toBeRemoved = x.Pages.Where(y => @event.RemovedPages.Contains(y.PageId)).ToList();

				foreach (var page in toBeRemoved)
				{
					x.Pages.Remove(page);
				}
			});
		}

		public void Process(NewPagesAdded @event)
		{
			_inventory.CreateOrUpdate(@event.AggregateId, x =>
			{
				foreach (var id in @event.AddedPages)
				{
					x.Pages.Add(new SiteInventoryPageInfo
					{
						FirstSeen = @event.TimeOfAddition,
						PageId = id
					});
				}
			});
		}
	}
}