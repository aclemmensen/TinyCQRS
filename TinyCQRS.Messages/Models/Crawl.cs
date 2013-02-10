using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TinyCQRS.Contracts.Models
{
	public class Crawl : Entity
	{
		public virtual Guid SiteId { get; set; }
		public virtual Site Site { get; set; }

		public DateTime? OrderTime { get; set; }
		public DateTime? StartTime { get; set; }
		public DateTime? CompletionTime { get; set; }

		public int NewPages { get; set; }
		public int RemovedPages { get; set; }
		public int ChangedPages { get; set; }
		public int UnchangedPages { get; set; }

		public CrawlStatus Status { get; set; }

		private ICollection<PageCheck> _checks;
		public virtual ICollection<PageCheck> Checks
		{
			get { return _checks ?? (_checks = new Collection<PageCheck>()); }
			set { _checks = value; }
		}
	}

	public enum CrawlStatus
	{
		None = 0,
		Ordered,
		InProgess,
		Completed
	}
}