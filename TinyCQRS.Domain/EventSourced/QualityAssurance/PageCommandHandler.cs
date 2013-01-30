using System;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages.Commands;

namespace TinyCQRS.Domain.EventSourced.QualityAssurance
{
	public class PageCommandHandler :
		IHandle<AddNewResourceToPage>,
		IHandle<AddExistingResourceToPage>,
		IHandle<CreatePage>,
		IHandle<UpdatePageContent>
	{
		private readonly IEventedRepository<PageAggregate> _repository;

		public PageCommandHandler(IEventedRepository<PageAggregate> repository)
		{
			_repository = repository;
		}

		public void Handle(AddExistingResourceToPage command)
		{
			var page = _repository.GetById(command.AggregateId);
			page.AddResource(command.ResourceId);
			_repository.Save(page);
		}

		public void Handle(CreatePage command)
		{
			var page = new PageAggregate(command.AggregateId, command.Url, command.Content);
			_repository.Save(page);
		}

		public void Handle(AddNewResourceToPage command)
		{
			var page = _repository.GetById(command.AggregateId);
			page.AddResource(Guid.NewGuid());
			_repository.Save(page);
		}

		public void Handle(UpdatePageContent command)
		{
			var page = _repository.GetById(command.AggregateId);
			page.ChangeContent(command.NewContent);
			_repository.Save(page);
		}
	}
}