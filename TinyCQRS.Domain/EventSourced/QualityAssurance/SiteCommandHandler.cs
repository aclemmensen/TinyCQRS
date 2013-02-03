using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages.Commands;

namespace TinyCQRS.Domain.EventSourced.QualityAssurance
{
	public class SiteCommandHandler :
		IHandle<CreateNewSite>,
		IHandle<CreateNewPage>,
		IHandle<UpdatePageContent>
	{
		private readonly IRepository<SiteAggregate> _repository;

		public SiteCommandHandler(IRepository<SiteAggregate> repository)
		{
			_repository = repository;
		}

		public void Handle(CreateNewSite command)
		{
			var site = new SiteAggregate(command.AggregateId, command.Name, command.Root);
			_repository.Save(site);
		}

		public void Handle(CreateNewPage command)
		{
			var site = _repository.GetById(command.AggregateId);
			site.AddNewPage(command.PageId, command.Url, command.Content);
			_repository.Save(site);
		}

		public void Handle(UpdatePageContent command)
		{
			var site = _repository.GetById(command.AggregateId);
			site.UpdatePageContent(command.PageId, command.NewContent);
			_repository.Save(site);
		}
	}
}