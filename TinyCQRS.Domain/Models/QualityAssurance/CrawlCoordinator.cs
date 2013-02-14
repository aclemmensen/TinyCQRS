using TinyCQRS.Contracts.Commands;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Domain.Models.QualityAssurance
{
	public class CrawlCoordinator :
		IHandle<OrderCrawl>,
		IHandle<OrderPageCheck>,
		IHandle<StartCrawl>,
		IHandle<RegisterCheckWithoutChange>,
		IHandle<RegisterNewPage>,
		IHandle<RegisterPageContentChange>,
		IHandle<MarkCrawlComplete>
	{
		private readonly IRepository<Crawl> _crawls;
		private readonly IBlobStorage _blobs;

		public CrawlCoordinator(IRepository<Crawl> crawls, IBlobStorage blobs)
		{
			_crawls = crawls;
			_blobs = blobs;
		}

		public void Handle(OrderCrawl command)
		{
			var crawl = new Crawl(command.AggregateId, command.SiteId, command.TimeOfOrder);
			_crawls.Save(crawl);
		}

		public void Handle(OrderPageCheck command)
		{
			throw new System.NotImplementedException();
		}

		public void Handle(StartCrawl command)
		{
			var crawl = _crawls.GetById(command.AggregateId);
			crawl.StartCrawl(command.CrawlerName, command.StartTime);
			_crawls.Save(crawl);
		}

		public void Handle(RegisterCheckWithoutChange command)
		{
			var crawl = _crawls.GetById(command.AggregateId);
			crawl.PageCheckedWithoutChange(command.PageId, command.TimeOfCheck);
			_crawls.Save(crawl);
		}

		public void Handle(RegisterNewPage command)
		{
			var crawl = _crawls.GetById(command.AggregateId);
			crawl.AddNewPage(command.PageId, command.Url, command.Content, command.TimeOfCreation, _blobs);
			_crawls.Save(crawl);
		}

		public void Handle(RegisterPageContentChange command)
		{
			var crawl = _crawls.GetById(command.AggregateId);
			crawl.UpdatePageContent(command.PageId, command.NewContent, command.TimeOfChange, _blobs);
			_crawls.Save(crawl);
		}

		public void Handle(MarkCrawlComplete command)
		{
			var crawl = _crawls.GetById(command.AggregateId);
			crawl.MarkCompleted(command.TimeOfCompletion, command.MissingPages);
			_crawls.Save(crawl);
		}

	}
}