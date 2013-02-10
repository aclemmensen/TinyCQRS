using System;

namespace TinyCQRS.Contracts.Models
{
	//public class CrawlJob : Dto
	//{
	//	public virtual Guid SiteId { get; set; }
	//	public virtual Site Site { get; set; }

	//	public virtual DateTime StartTime { get; set; }

	//	private Collection<CrawlRecord> _records;
	//	public virtual ICollection<CrawlRecord> Records { get { return _records ?? (_records = new Collection<CrawlRecord>()); } }
	//}

	//public class CrawlRecord : Dto
	//{
	//	public DateTime TimeOfChange { get; set; }

	//	public virtual Guid CrawlJobId { get; set; }
	//	public virtual CrawlJob CrawlJob { get; set; }

	//	public virtual Guid PageId { get; set; }
	//	public virtual Page Page { get; set; }
	//}

	public class PageCheck : Dto
	{
		public DateTime TimeOfCheck { get; set; }

		public virtual Guid PageId { get; set; }
		public virtual Page Page { get; set; }

		public virtual Guid CrawlId { get; set; }
		public virtual Crawl Crawl { get; set; }
	}
}