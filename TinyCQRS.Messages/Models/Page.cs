using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TinyCQRS.Contracts.Models
{
	public class Page : Entity
	{
		public virtual string Url { get; set; }
		
		public virtual string Content { get; set; }
		
		public virtual Guid SiteId { get; set; }

		public DateTime? FirstSeen { get; set; }
		public DateTime? LastChecked { get; set; }

		private ICollection<PageCheck> _checks;
		public virtual ICollection<PageCheck> Checks
		{
			get { return _checks ?? (_checks = new Collection<PageCheck>()); }
			set { _checks = value; }
		}
	}
}