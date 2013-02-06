using System;
using System.Collections.Generic;
using TinyCQRS.Messages.Commands;
using TinyCQRS.ReadModel.Interfaces;
using TinyCQRS.ReadModel.Model;

namespace TinyCQRS.Client
{
	public class SiteCrawlService : ISiteCrawlService
	{
		private readonly ICommandDispatcher _dispatcher;
		private readonly IReadModelRepository<Site> _read;

		public SiteCrawlService(ICommandDispatcher dispatcher, IReadModelRepository<Site> read)
		{
			_dispatcher = dispatcher;
			_read = read;
		}

		#region WRITE

		public void CreateNewSite(CreateNewSite command)
		{
			_dispatcher.Dispatch(command);
		}

		public void StartCrawl(StartCrawl command)
		{
			_dispatcher.Dispatch(command);
		}

		public void RegisterNewPage(RegisterNewPage command)
		{
			_dispatcher.Dispatch(command);
		}

		public void UpdatePageContent(UpdatePageContent command)
		{
			_dispatcher.Dispatch(command);
		}

		public void PageCheckedWithoutChanges(RegisterPageCheck command)
		{
			_dispatcher.Dispatch(command);
		}


		#endregion

		#region READ

		public Site GetSite(Guid siteId)
		{
			return _read.Get(siteId);
		}

		public IEnumerable<Page> GetPagesFor(Guid siteId)
		{
			return _read.Get(siteId).Pages;
		}

		public CrawlSpec GetCrawlInfoFor(Guid siteId)
		{
			var site = _read.Get(siteId);

			return new CrawlSpec(site);
		}

		#endregion
	}
}