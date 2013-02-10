using TinyCQRS.Contracts.Commands;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Domain.Models.QualityAssurance
{
	public class SiteCommandHandler :
		IHandle<CreateNewSite>
	{
		private readonly IRepository<SiteAggregate> _sites;

		public SiteCommandHandler(IRepository<SiteAggregate> sites)
		{
			_sites = sites;
		}

		public void Handle(CreateNewSite command)
		{
			var site = new SiteAggregate(command.AggregateId, command.Name, command.Root);
			_sites.Save(site);
		}
	}
}