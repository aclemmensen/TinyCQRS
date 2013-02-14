using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TinyCQRS.Contracts.Models
{
	public class PageComponents
	{
		public IEnumerable<Image> Images { get; set; }
		public IEnumerable<Heading> Headings { get; set; }
		public IEnumerable<Asset> Assets { get; set; }
		public string TextContent { get; set; }

		public PageComponents()
		{
			Images = new Collection<Image>();
			Headings = new Collection<Heading>();
			Assets = new Collection<Asset>();
		}
	}

	public class Image
	{
		public string Url { get; set; }
		public string Alt { get; set; }
		public string Title { get; set; }
	}

	public class Heading
	{
		public int Level { get; set; }
		public string Text { get; set; }
	}

	public class Asset
	{
		public string Url { get; set; }
		public AssetType Type { get; set; }
	}

	public enum AssetType
	{
		Unknown = 0,
		JavaScript,
		Link,
		Document,
		Media
	}
}