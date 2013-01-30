using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Model
{
	public class PageDto : IDto
	{
		public Guid Id { get; set; }

		public string Url { get; set; }
		
		public string Content { get; set; }

		private Collection<ResourceDto> _resources; 
		public ICollection<ResourceDto> Resources { get { return _resources ?? (_resources = new Collection<ResourceDto>()); } }

		public PageDto() { }

		public PageDto(Guid id)
		{
			Id = id;
		}
	}

	public class SiteDto : IDto
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string Root { get; set; }

		private List<PageDto> _pages;
		public List<PageDto> Pages { get { return _pages ?? (_pages = new List<PageDto>()); } }
	}
}