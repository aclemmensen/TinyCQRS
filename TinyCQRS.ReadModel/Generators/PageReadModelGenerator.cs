using TinyCQRS.Messages;
using TinyCQRS.Messages.Events;
using TinyCQRS.ReadModel.Interfaces;
using TinyCQRS.ReadModel.Model;

namespace TinyCQRS.ReadModel.Generators
{
	public class PageReadModelGenerator :
		IConsume<PageCreated>,
		IConsume<PageContentChanged>,
		IConsume<ResourceAdded>
	{
		private readonly IDtoRepository<PageDto> _repository;
		private readonly IDtoRepository<ResourceDto> _resourceRepository;

		public PageReadModelGenerator(IDtoRepository<PageDto> repository, IDtoRepository<ResourceDto> resourceRepository)
		{
			_repository = repository;
			_resourceRepository = resourceRepository;
		}

		public void Process(PageCreated @event)
		{
			var page = new PageDto(@event.AggregateId)
			{
				Url = @event.Url,
				Content = @event.Content
			};

			_repository.Save(page);
		}

		public void Process(ResourceAdded @event)
		{
			var page = _repository.GetById(@event.AggregateId);
			var resource = _resourceRepository.GetById(@event.ResourceId);

			page.Resources.Add(resource);

			_repository.Save(page);
		}

		public void Process(PageContentChanged @event)
		{
			var page = _repository.GetById(@event.AggregateId);
			page.Content = @event.Content;
			_repository.Save(page);
		}
	}
}