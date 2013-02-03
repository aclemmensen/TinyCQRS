using TinyCQRS.Messages;
using TinyCQRS.Messages.Events;
using TinyCQRS.ReadModel.Interfaces;
using TinyCQRS.ReadModel.Model;

namespace TinyCQRS.ReadModel.Generators
{
	public class SiteReadModelGenerator : 
		IConsume<SiteCreatedEvent>
	{
		private readonly IReadModelRepository<Site> _siteRepository;
		private readonly IReadModelRepository<Page> _pageRepository;

		public SiteReadModelGenerator(IReadModelRepository<Site> siteRepository, IReadModelRepository<Page> pageRepository)
		{
			_siteRepository = siteRepository;
			_pageRepository = pageRepository;
		}

		public void Process(SiteCreatedEvent @event)
		{
			var site = _siteRepository.Create();
			
			site.Id = @event.AggregateId;
			site.Name = @event.Name;
			site.Root = @event.Root;

			_siteRepository.Add(site);
			//_siteRepository.Commit();
		}

		//public void Process(PageAddedEvent @event)
		//{
		//	var site = _siteRepository.Get(@event.AggregateId);
		//	var page = _pageRepository.Get(@event.PageId);
			
		//	site.Pages.Add(page);

		//	_siteRepository.Update(site);
		//	_siteRepository.Commit();
		//}
	}
}