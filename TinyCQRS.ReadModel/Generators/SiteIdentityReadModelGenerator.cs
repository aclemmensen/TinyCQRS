using System.Linq;
using TinyCQRS.Contracts;
using TinyCQRS.Contracts.Events;
using TinyCQRS.Contracts.Models;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Generators
{
	public class SiteIdentityReadModelGenerator :
		IConsume<SiteCreatedEvent>,
		IConsume<CrawlCompleted>
	{
		private readonly IReadModelRepository<SiteIdentity> _read;
		private readonly IReadModelRepository<Crawl> _crawls;

		public SiteIdentityReadModelGenerator(IReadModelRepository<SiteIdentity> read, IReadModelRepository<Crawl> crawls)
		{
			_read = read;
			_crawls = crawls;
		}

		public void Process(SiteCreatedEvent @event)
		{
			var site = _read.Create();

			site.GlobalId = @event.AggregateId;
			site.Name = @event.Name;
			site.Root = @event.Root;
			site.PageCount = 0;

			_read.Add(site);
			_read.Commit();
		}

		public void Process(CrawlCompleted @event)
		{
			var crawl = _crawls.Get(@event.AggregateId);
			var site = _read.Get(crawl.SiteId);

			var total = @event.NewPages.Count() + @event.UnchangedPages.Count() + @event.ChangedPages.Count();

			site.PageCount = total;

			_read.Update(site);
			_read.Commit();
		}
	}
}