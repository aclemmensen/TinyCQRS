using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Model
{
	public class Site : Entity
	{
		public virtual string Name { get; set; }

		public virtual string Root { get; set; }

		private Collection<Crawl> _crawls;
		public ICollection<Crawl> Crawls { get { return _crawls ?? (_crawls = new Collection<Crawl>()); } }

		private Collection<Page> _pages;
		public virtual ICollection<Page> Pages { get { return _pages ?? (_pages = new Collection<Page>()); } }
	}

	public class Crawl : Entity
	{
		public virtual Guid SiteId { get; set; }
		public virtual Site Site { get; set; }

		public DateTime StartTime { get; set; }

		private Collection<PageCheck> _checks;
		public ICollection<PageCheck> Checks { get { return _checks ?? (_checks = new Collection<PageCheck>()); } }
	}
}