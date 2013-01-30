using TinyCQRS.Messages;
using TinyCQRS.Messages.Events;
using TinyCQRS.ReadModel.Interfaces;
using TinyCQRS.ReadModel.Model;

namespace TinyCQRS.ReadModel.Generators
{
	public class SiteReadModelGenerator : 
		IConsume<SiteCreatedEvent>,
		IConsume<PageAddedEvent>
	{
		private readonly IDtoRepository<SiteDto> _siteRepository;
		private readonly IDtoRepository<PageDto> _pageRepository;

		public SiteReadModelGenerator(IDtoRepository<SiteDto> siteRepository, IDtoRepository<PageDto> pageRepository)
		{
			_siteRepository = siteRepository;
			_pageRepository = pageRepository;
		}

		public void Process(SiteCreatedEvent @event)
		{
			var site = new SiteDto
			{
				Id = @event.AggregateId,
				Name = @event.Name,
				Root = @event.Root
			};

			_siteRepository.Save(site);
		}

		public void Process(PageAddedEvent @event)
		{
			var site = _siteRepository.GetById(@event.AggregateId);
			var page = _pageRepository.GetById(@event.PageId);
			
			site.Pages.Add(page);

			_siteRepository.Save(site);
		}
	}
}