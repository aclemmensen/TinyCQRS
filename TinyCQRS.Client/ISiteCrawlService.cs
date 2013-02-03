using System;
using System.Collections.Generic;
using TinyCQRS.ReadModel.Model;

namespace TinyCQRS.Client
{
	public interface ISiteCrawlService
	{
		void StartCrawl(Guid crawlId, Guid siteId);

		void CreateNewSite(Guid id, string name, string root);

		void AddNewPage(Guid crawlId, Guid pageId, string url, string content);

		void UpdatePageContent(Guid crawlId, Guid pageId, string newContent);

		void PageCheckedWithoutChanges(Guid crawlId, Guid pageId, DateTime timeOfCheck);

		Site GetSite(Guid siteId);

		IEnumerable<Page> GetPagesFor(Guid siteId);

		CrawlSpec GetCrawlInfoFor(Guid siteId);
	}
}