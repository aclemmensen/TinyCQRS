using TinyCQRS.Contracts;
using TinyCQRS.Contracts.Events;
using TinyCQRS.Contracts.Models;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Generators
{
	public class PageReadModelGenerator :
		IConsume<PageContentChanged>,
		IConsume<PageChecked>
	{
		private readonly IReadModelRepository<Page> _pages;

		public PageReadModelGenerator(IReadModelRepository<Page> pages)
		{
			_pages = pages;
		}

		public void Process(PageContentChanged @event)
		{
			var page = _pages.Get(@event.AggregateId);
			//page.Content = @event.Content;
			_pages.Update(page);
			_pages.Commit();
		}

		public void Process(PageChecked @event)
		{
			//var page = _pages.Get(@event.PageId);
			
			//if (!page.FirstSeen.HasValue)
			//{
			//	page.FirstSeen = @event.TimeOfCheck;
			//}

			//page.LastChecked = @event.TimeOfCheck;

			//page.Checks.Add(new PageCheck
			//{
			//	Page = page,
			//	PageId = @event.PageId,
			//	TimeOfCheck = @event.TimeOfCheck,
			//	CrawlId = @event.AggregateId
			//});
			
			//_pages.Update(page);
			//_pages.Commit();
		}
	}
}