using TinyCQRS.Messages;
using TinyCQRS.Messages.Events;
using TinyCQRS.ReadModel.Interfaces;
using TinyCQRS.ReadModel.Model;

namespace TinyCQRS.ReadModel.Generators
{
	public class PageReadModelGenerator :
		IConsume<PageCreated>,
		IConsume<PageContentChanged>,
		IConsume<PageChecked>
	{
		private readonly IReadModelRepository<Page> _pages;

		public PageReadModelGenerator(IReadModelRepository<Page> pages)
		{
			_pages = pages;
		}

		public void Process(PageCreated @event)
		{
			var page = new Page
			{
				Id = @event.PageId,
				Url = @event.Url,
				Content = @event.Content,
				SiteId = @event.AggregateId
			};

			_pages.Add(page);
			_pages.Commit();
		}

		public void Process(PageContentChanged @event)
		{
			var page = _pages.Get(@event.AggregateId);
			page.Content = @event.Content;
			_pages.Add(page);
			_pages.Commit();
		}

		public void Process(PageChecked @event)
		{
			var page = _pages.Get(@event.PageId);
			
			if (!page.FirstSeen.HasValue)
			{
				page.FirstSeen = @event.TimeOfCheck;
			}

			page.LastChecked = @event.TimeOfCheck;
			
			_pages.Commit();
		}
	}
}