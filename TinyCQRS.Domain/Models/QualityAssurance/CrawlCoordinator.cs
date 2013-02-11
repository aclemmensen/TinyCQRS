using System;
using System.Collections.Generic;
using TinyCQRS.Contracts;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Events;
using TinyCQRS.Contracts.Services;
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
		IHandle<MarkCrawlComplete>,
		IConsume<CrawlCompleted>
		//IConsume<SpellcheckCompleted>
	{
		private readonly ISagaRepository<CrawlSaga> _crawls;
		private readonly IRepository<SiteAggregate> _sites;
		private readonly IBlobService _blobs;

		public CrawlCoordinator(ISagaRepository<CrawlSaga> crawls, IRepository<SiteAggregate> sites, IBlobService blobs)
		{
			_crawls = crawls;
			_sites = sites;
			_blobs = blobs;
		}

		public void Handle(OrderCrawl command)
		{
			var crawl = new CrawlSaga(command.AggregateId, command.SiteId, command.TimeOfOrder);
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

		public void Process(CrawlCompleted @event)
		{
			var site = _sites.GetById(@event.SiteId);

			site.AddPages(@event.NewPages, @event.TimeOfCompletion);
			site.UpdatePages(@event.ChangedPages, @event.TimeOfCompletion);
			site.RemovePages(@event.MissingPages, @event.TimeOfCompletion);

			_sites.Save(site);
		}

		public void Process(SpellcheckCompleted @event)
		{
			throw new System.NotImplementedException();
		}
	}
}