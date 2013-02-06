using System;
using System.Collections.Generic;
using TinyCQRS.Messages.Commands;
using TinyCQRS.ReadModel.Model;

namespace TinyCQRS.Client
{
	public interface ISiteCrawlService
	{
		void CreateNewSite(CreateNewSite command);

		void StartCrawl(StartCrawl command);

		void RegisterNewPage(RegisterNewPage command);

		void UpdatePageContent(UpdatePageContent command);

		void PageCheckedWithoutChanges(RegisterPageCheck command);

		Site GetSite(Guid siteId);

		IEnumerable<Page> GetPagesFor(Guid siteId);

		CrawlSpec GetCrawlInfoFor(Guid siteId);
	}
}