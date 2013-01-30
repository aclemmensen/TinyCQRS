using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages.Commands;

namespace TinyCQRS.Domain.EventSourced.QualityAssurance
{
	public class SiteCommandHandler :
		IHandle<CreateNewSite>,
		IHandle<AddPageToSite>
	{
		private readonly IEventedRepository<SiteAggregate> _repository;

		public SiteCommandHandler(IEventedRepository<SiteAggregate> repository)
		{
			_repository = repository;
		}

		public void Handle(CreateNewSite command)
		{
			var site = new SiteAggregate(command.AggregateId, command.Name, command.Root);
			_repository.Save(site);
		}

		public void Handle(AddPageToSite command)
		{
			var site = _repository.GetById(command.AggregateId);
			site.AddPage(command.PageId);

			_repository.Save(site);
		}
	}
}