using TinyCQRS.Messages;
using TinyCQRS.Messages.Events;
using TinyCQRS.ReadModel.Interfaces;
using TinyCQRS.ReadModel.Model;

namespace TinyCQRS.ReadModel.Generators
{
	public class SiteReadModelGenerator : 
		IConsume<SiteCreatedEvent>,
		IConsume<PageCreated>
	{
		private readonly IReadModelRepository<Site> _sites;

		public SiteReadModelGenerator(IReadModelRepository<Site> sites)
		{
			_sites = sites;
		}

		public void Process(SiteCreatedEvent @event)
		{
			var site = _sites.Create();
			
			site.Id = @event.AggregateId;
			site.Name = @event.Name;
			site.Root = @event.Root;

			_sites.Add(site);
			_sites.Commit();
		}

		public void Process(PageCreated @event)
		{
			var site = _sites.Get(@event.SiteId);

			var page = new Page
			{
				Id = @event.PageId,
				Content = @event.Content,
				FirstSeen = @event.TimeOfCreation,
				Url = @event.Url,
				SiteId = @event.SiteId
			};

			page.Checks.Add(new PageCheck
			{
				Page = page,
				PageId = @event.PageId,
				TimeOfCheck = @event.TimeOfCreation,
				CrawlId = @event.AggregateId
			});

			site.Pages.Add(page);

			_sites.Update(site);
			_sites.Commit();
			
		}
	}
}