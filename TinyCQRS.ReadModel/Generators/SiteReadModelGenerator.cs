using System;
using TinyCQRS.Contracts;
using TinyCQRS.Contracts.Events;
using TinyCQRS.Contracts.Models;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Generators
{
	public class SiteReadModelGenerator : 
		IConsume<SiteCreatedEvent>
	{
		private readonly IReadModelRepository<Site> _sites;

		public SiteReadModelGenerator(IReadModelRepository<Site> sites)
		{
			_sites = sites;
		}

		public void Process(SiteCreatedEvent @event)
		{
			_sites.CreateOrUpdate(@event.AggregateId, x =>
			{
				x.Id = @event.AggregateId;
				x.Name = @event.Name;
				x.Root = @event.Root;
			});
		}
	}
}