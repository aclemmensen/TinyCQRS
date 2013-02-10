using TinyCQRS.Contracts.Commands;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Domain.Models.QualityAssurance
{
	public class CrawlJobCommandHandler :
		IHandle<StartCrawl>,
		IHandle<RegisterNoChangeCheck>,
		IHandle<RegisterNewPage>,
		IHandle<RegisterPageContentChange>,
		IHandle<MarkCrawlComplete>
	{
		private readonly IRepository<CrawlAggregate> _repository;

		public CrawlJobCommandHandler(IRepository<CrawlAggregate> repository)
		{
			_repository = repository;
		}

		public void Handle(StartCrawl command)
		{
			var crawl = _repository.GetById(command.AggregateId);
			crawl.StartCrawl(command.CrawlerName, command.StartTime);
			_repository.Save(crawl);
		}

		public void Handle(RegisterNoChangeCheck command)
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

		public void Handle(RegisterPageContentChange command)
		{
			var crawl = _repository.GetById(command.AggregateId);
			crawl.RegisterPageContentChange(command.PageId, command.NewContent, command.TimeOfChange);
			_repository.Save(crawl);
		}

		public void Handle(MarkCrawlComplete command)
		{
			var crawl = _repository.GetById(command.AggregateId);
			crawl.MarkCompleted(command.TimeOfCompletion);
			_repository.Save(crawl);
		}
	}
}