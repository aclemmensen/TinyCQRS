using System;
using System.Collections.Generic;
using System.Linq;
using TinyCQRS.ReadModel.Model;

namespace TinyCQRS.Client
{
	public class CrawlSpec
	{
		public Guid SiteId { get; set; }
		public string Root { get; set; }

		private readonly List<PageInfo> _pageInfo;
		public IEnumerable<PageInfo> Pages { get { return _pageInfo; } }

		public CrawlSpec(Site site)
		{
			SiteId = site.Id;
			Root = site.Root;

			_pageInfo = site.Pages.Select(page => new PageInfo
			{
				PageId = page.Id,
				Url = page.Url,
				ContentHash = HashingHelper.Hash(page.Content)
			}).ToList();
		}
	}
}