using System;

namespace TinyCQRS.Contracts.Models
{
	public class PageInfo
	{
		public Guid PageId { get; set; }
		public string Url { get; set; }
		public string ContentHash { get; set; }
	}
}