using System.Linq;
using TinyCQRS.Contracts;
using TinyCQRS.Contracts.Events;
using TinyCQRS.Contracts.Models;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Generators
{
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