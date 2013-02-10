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
			var site = _sites.Create();
			
			site.GlobalId = @event.AggregateId;
			site.Name = @event.Name;
			site.Root = @event.Root;

			_sites.Add(site);
			_sites.Commit();
		}
	}
}