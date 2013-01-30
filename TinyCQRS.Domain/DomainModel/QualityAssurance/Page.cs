using System;

namespace TinyCQRS.Domain.DomainModel.QualityAssurance
{
	public class Page : Entity
	{
		private string _url;
		private string _content;

		public Page(Guid id, string url, string content)
		{
			Id = id;
			_url = url;
			_content = content;
		}
	}

	public class Site : Entity
	{
		private string _root;

		public Site(Guid id, string root)
		{
			Id = id;
			_root = root;
		}
	}
}