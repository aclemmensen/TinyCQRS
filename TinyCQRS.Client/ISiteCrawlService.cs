using System;
using System.Collections.Generic;
using TinyCQRS.ReadModel.Model;

namespace TinyCQRS.Client
{
	public interface ISiteCrawlService
	{
		void CreateNewSite(Guid id, string name, string root);

		void AddNewPage(Guid siteId, Guid pageId, string url, string content);

		void UpdatePageContent(Guid siteId, Guid pageId, string newContent);

		SiteDto GetSite(Guid siteId);

		IEnumerable<PageDto> GetPagesFor(Guid siteId);

		CrawlSpec GetCrawlInfoFor(Guid siteId);
	}
}