using System.Collections.Generic;
using System.Collections.ObjectModel;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Model
{
	public class Site : Dto
	{
		public virtual string Name { get; set; }

		public virtual string Root { get; set; }

		private Collection<Page> _pages;
		public virtual ICollection<Page> Pages { get { return _pages ?? (_pages = new Collection<Page>()); } }

		private Collection<CrawlJob> _crawls;
		public virtual ICollection<CrawlJob> Crawls { get { return _crawls ?? (_crawls = new Collection<CrawlJob>()); } }
	}
}