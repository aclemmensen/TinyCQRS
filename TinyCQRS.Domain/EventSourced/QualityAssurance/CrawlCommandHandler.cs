using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages.Commands;

namespace TinyCQRS.Domain.EventSourced.QualityAssurance
{
	public class SiteCommandHandler :
		IHandle<CreateNewSite>
	{
		private readonly IRepository<SiteAggregate> _sites;

		public SiteCommandHandler(IRepository<SiteAggregate> sites)
		{
			_sites = sites;
		}

		public void Handle(CreateNewSite command)
		{
			var site = new SiteAggregate(command.AggregateId, command.Name, command.Root);
			_sites.Save(site);
		}
	}

	public class CrawlCommandHandler :
		IHandle<StartCrawl>,
		IHandle<RegisterPageCheck>,
		IHandle<RegisterNewPage>,
		IHandle<UpdatePageContent>
	{
		private readonly IRepository<CrawlAggregate> _repository;

		public CrawlCommandHandler(IRepository<CrawlAggregate> repository)
		{
			_repository = repository;
		}

		public void Handle(StartCrawl command)
		{
			var crawl = new CrawlAggregate(command.AggregateId, command.SiteId, command.StartTime);
			_repository.Save(crawl);
		}


		public void Handle(RegisterPageCheck command)
		{
			var crawl = _repository.GetById(command.AggregateId);
			crawl.RegisterPageCheck(command.PageId, command.TimeOfCheck);
			_repository.Save(crawl);
		}

		public void Handle(RegisterNewPage command)
		{
			var crawl = _repository.GetById(command.AggregateId);
			crawl.RegisterNewPage(command.PageId, command.Url, command.Content, command.TimeOfCreation);
			_repository.Save(crawl);
		}

		public void Handle(UpdatePageContent command)
		{
			var crawl = _repository.GetById(command.AggregateId);
			crawl.RegisterPageContentChange(command.PageId, command.NewContent, command.TimeOfChange);
			_repository.Save(crawl);
		}
	}

	//public class PageCommandHandler :
	//	IHandle<UpdatePageContent>
	//{
	//	private readonly IRepository<PageAggregate> _repository;

	//	public PageCommandHandler(IRepository<PageAggregate> repository)
	//	{
	//		_repository = repository;
	//	}

	//	public void Handle(UpdatePageContent command)
	//	{
	//		var page = _repository.GetById(command.AggregateId);
	//		page.UpdatePageContent(command.NewContent, command.TimeOfChange);
	//		_repository.Save(page);
	//	}
	//}
}