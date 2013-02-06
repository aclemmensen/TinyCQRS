using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Model
{
	public class Page : Entity
	{
		public virtual string Url { get; set; }
		
		public virtual string Content { get; set; }
		
		public virtual Guid SiteId { get; set; }

		public DateTime? FirstSeen { get; set; }
		public DateTime? LastChecked { get; set; }

		private Collection<PageCheck> _checks;
		public ICollection<PageCheck> Checks { get { return _checks ?? (_checks = new Collection<PageCheck>()); } }
	}
}