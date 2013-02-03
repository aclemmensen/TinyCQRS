using System;
using System.Collections.ObjectModel;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Model
{
	public class Page : Dto
	{
		public virtual string Url { get; set; }
		
		public virtual string Content { get; set; }
		
		public virtual Guid SiteId { get; set; }

		private Collection<CrawlRecord> _records;
		public virtual Collection<CrawlRecord> CrawlRecords { get { return _records ?? (_records = new Collection<CrawlRecord>()); } }

		public DateTime? FirstSeen { get; set; }
		public DateTime? LastChecked { get; set; }

		//private Collection<ResourceDto> _resources; 
		//public ICollection<ResourceDto> Resources { get { return _resources ?? (_resources = new Collection<ResourceDto>()); } }
	}
}