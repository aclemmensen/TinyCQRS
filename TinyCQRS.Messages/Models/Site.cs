using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TinyCQRS.Contracts.Models
{
	public class Site : Entity
	{
		public virtual string Name { get; set; }

		public virtual string Root { get; set; }

		private ICollection<Crawl> _crawls;
		public virtual ICollection<Crawl> Crawls
		{
			get { return _crawls ?? (_crawls = new Collection<Crawl>()); }
			set { _crawls = value; }
		}

		private ICollection<Page> _pages;
		public virtual ICollection<Page> Pages
		{
			get { return _pages ?? (_pages = new Collection<Page>()); } 
			set { _pages = value; }
		}
	}

	public class SiteIdentity : Entity
	{
		public string Name { get; set; }
		public string Root { get; set; }
		public int PageCount { get; set; }
	}

	public class PageIdentity : Entity
	{
		public string Title { get; set; }
		public string Url { get; set; }
		public float Score { get; set; }
	}

	public class SiteOverview : Entity
	{
		public int PageCount { get; set; }

		public ErrorCount BrokenLinksCount { get; set; }
		public ErrorCount MisspellingsCount { get; set; }

		public ICollection<PageIdentity> PriorityPages { get; set; }
		public ICollection<CrawlStatusItem> History { get; set; }

		public DateTime? LastCrawl { get; set; }
		public DateTime? NextCrawl { get; set; }

		public SiteOverview()
		{
			PriorityPages = new Collection<PageIdentity>();
			History = new Collection<CrawlStatusItem>();
		}
	}

	public class CrawlStatusItem : ValueObject
	{
		public DateTime Time { get; set; }
		public int BrokenLinks { get; set; }
		public int Misspellings { get; set; }
	}

	public class ErrorCount : ValueObject
	{
		public int Total { get; set; }
		public int New { get; set; }
		public int Affecting { get; set; }

		public ErrorCount() { }

		public ErrorCount(int total, int @new, int affecting)
		{
			Total = total;
			New = @new;
			Affecting = affecting;
		}
	}

}