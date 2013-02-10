﻿using System;
﻿using System.Collections.Generic;
﻿using System.Linq;
﻿using TinyCQRS.Contracts;
﻿using TinyCQRS.Contracts.Events;
﻿using TinyCQRS.Contracts.Models;
﻿using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Generators
{
	public class CrawlReadModelGenerator :
		IConsume<CrawlOrdered>,
		IConsume<CrawlStarted>,
		IConsume<CrawlCompleted>
	{
		private readonly IReadModelRepository<Crawl> _crawls;
		private readonly IReadModelRepository<Page> _pages;

		public CrawlReadModelGenerator(IReadModelRepository<Crawl> crawls, IReadModelRepository<Page> pages)
		{
			_crawls = crawls;
			_pages = pages;
		}

		public void Process(CrawlOrdered @event)
		{
			var crawl = _crawls.Create();

			crawl.GlobalId = @event.AggregateId;
			crawl.SiteId = @event.SiteId;
			crawl.OrderTime = @event.TimeOfOrder;
			crawl.Status = CrawlStatus.Ordered;

			_crawls.Add(crawl);
			_crawls.Commit();
		}

		public void Process(CrawlStarted @event)
		{
			var crawl = _crawls.Get(@event.AggregateId);

			crawl.StartTime = @event.StartTime;
			crawl.Status = CrawlStatus.InProgess;

			_crawls.Update(crawl);
			_crawls.Commit();
		}

		public void Process(CrawlCompleted @event)
		{
			var crawl = _crawls.Get(@event.AggregateId);

			crawl.Status = CrawlStatus.Completed;
			crawl.CompletionTime = @event.TimeOfCompletion;

			var last = _crawls
				.Where(x => 
					x.SiteId == crawl.SiteId && 
					x.GlobalId != @event.AggregateId && 
					x.Status == CrawlStatus.Completed)
				.OrderByDescending(x => x.CompletionTime)
				.FirstOrDefault();

			var disappeared = new List<Guid>();

			if (last != null)
			{
				var ids = last.Checks.Select(x => x.PageId);

				var newIds = new List<Guid>(@event.ChangedPages);
				newIds.AddRange(@event.UnchangedPages);

				disappeared = ids.Except(newIds).ToList();

				var currentPages = _pages.Where(x => x.SiteId == crawl.SiteId);

				foreach (var page in currentPages.Where(x => disappeared.Contains(x.GlobalId)).ToList())
				{
					_pages.Delete(page);
				}

			}

			crawl.RemovedPages = disappeared.Count;
			crawl.ChangedPages = @event.ChangedPages.Count();
			crawl.NewPages = @event.NewPages.Count();
			crawl.UnchangedPages = @event.UnchangedPages.Count();

			_crawls.Update(crawl);
			_crawls.Commit();
		}
	}
}
