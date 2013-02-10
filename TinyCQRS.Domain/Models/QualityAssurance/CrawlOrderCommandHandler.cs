using TinyCQRS.Contracts.Commands;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Domain.Models.QualityAssurance
{
	public class CrawlOrderCommandHandler :
		IHandle<OrderCrawl>,
		IHandle<OrderPageCheck>
	{
		private readonly IRepository<CrawlAggregate> _crawls;
		private readonly IRepository<SiteAggregate> _sites;

		public CrawlOrderCommandHandler(IRepository<CrawlAggregate> crawls, IRepository<SiteAggregate> sites)
		{
			_crawls = crawls;
			_sites = sites;
		}

		public void Handle(OrderCrawl command)
		{
			var crawl = new CrawlAggregate(command.AggregateId, command.SiteId, command.TimeOfOrder);
			_crawls.Save(crawl);
		}

		public void Handle(OrderPageCheck command)
		{
			throw new System.NotImplementedException();
		}
	}
}