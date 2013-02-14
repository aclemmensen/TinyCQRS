using System;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Models;

namespace TinyCQRS.Contracts.Services
{
	public interface ICrawlService : IService
	{
		void StartCrawl(StartCrawl command);

		void RegisterNewPage(RegisterNewPage command);

		void UpdatePageContent(RegisterPageContentChange command);

		void PageCheckedWithoutChanges(RegisterCheckWithoutChange command);

		void MarkCrawlComplete(MarkCrawlComplete command);

		CrawlSpec GetCrawlInfoFor(Guid siteId);
	}
}