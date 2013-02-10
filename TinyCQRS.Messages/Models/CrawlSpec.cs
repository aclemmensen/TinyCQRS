using System;
using System.Collections.Generic;

namespace TinyCQRS.Contracts.Models
{
	public class CrawlSpec
	{
		public Guid SiteId { get; set; }
		public string Root { get; set; }

		public IEnumerable<PageInfo> Pages { get; set; }
	}
}