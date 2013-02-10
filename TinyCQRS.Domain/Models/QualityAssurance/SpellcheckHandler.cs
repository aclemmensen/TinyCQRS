using TinyCQRS.Contracts.Commands;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Domain.Models.QualityAssurance
{
	public class SpellcheckHandler :
		IHandle<SpellcheckPage>
	{
		private readonly IRepository<SiteAggregate> _sites;

		public SpellcheckHandler(IRepository<SiteAggregate> sites)
		{
			_sites = sites;
		}

		public void Handle(SpellcheckPage command)
		{
			throw new System.NotImplementedException();
		}
	}
}