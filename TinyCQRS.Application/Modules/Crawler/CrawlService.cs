using System;
using System.Linq;
using TinyCQRS.Contracts;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Models;
using TinyCQRS.Contracts.Services;
using TinyCQRS.Domain;
using TinyCQRS.Infrastructure;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.Application.Modules.Crawler
{
	public class CrawlService : ICrawlService
	{
		private readonly ICommandDispatcher _dispatcher;
		private readonly IReadModelRepository<Site> _read;

		public CrawlService(ICommandDispatcher dispatcher, IReadModelRepository<Site> read)
		{
			_dispatcher = dispatcher;
			_read = read;
		}

		public void StartCrawl(StartCrawl command)
		{
			Handle(command);
		}

		public void RegisterNewPage(RegisterNewPage command)
		{
			Handle(command);
		}

		public void UpdatePageContent(RegisterPageContentChange command)
		{
			Handle(command);
		}

		public void PageCheckedWithoutChanges(RegisterCheckWithoutChange command)
		{
			Handle(command);
		}

		public void MarkCrawlComplete(MarkCrawlComplete command)
		{
			Handle(command);
		}

		private void Handle<T>(T command) where T : Command
		{
			_dispatcher.Dispatch(command);
		}

		public CrawlSpec GetCrawlInfoFor(Guid siteId)
		{
			var site = _read.Get(siteId);

			return new CrawlSpec
			{
				Pages = site.Pages.Select(x => new PageInfo
				{
					ContentHash = HashingHelper.Hash(x.Content),
					PageId = x.Id,
					Url = x.Url
				}),
				Root = site.Root,
				SiteId = site.Id
			};
		}
	}
}