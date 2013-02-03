using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages.Commands;

namespace TinyCQRS.Domain.EventSourced.QualityAssurance
{
	public class CrawlCommandHandler :
		IHandle<StartCrawl>,
		IHandle<RegisterCheck>
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

		public void Handle(RegisterCheck command)
		{
			var crawl = _repository.GetById(command.AggregateId);
			crawl.FlagPageAsChecked(command.PageId, command.TimeOfCheck);
			_repository.Save(crawl);
		}
	}
}