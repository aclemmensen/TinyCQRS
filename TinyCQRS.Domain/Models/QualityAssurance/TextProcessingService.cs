using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TinyCQRS.Contracts.Models;

namespace TinyCQRS.Domain.Models.QualityAssurance
{
	public class TextProcessingService
	{
		private HashSet<IFilter> _filters;

		public TextProcessingService(IFilterFactory factory)
		{
			_filters = new HashSet<IFilter>(factory.CreateFilters());
		}

		public IEnumerable<T> Get<T>(string input)
		{
			var filter = _filters.OfType<IFilter<T>>().FirstOrDefault();

			return filter == null 
				? new List<T>() 
				: filter.Parse(input);
		}

		public string GetPlainText(string input)
		{
			return Regex.Replace(input, "<.*?>", string.Empty);
		}
	}

	public interface IFilter { }

	public interface IFilter<T> : IFilter
	{
		IEnumerable<T> Parse(string input);
	}

	public abstract class BaseFilter<T> : IFilter<T>
	{
		private readonly Regex _regex;

		protected BaseFilter(string pattern)
		{
			_regex = new Regex(pattern);
		}

		public IEnumerable<T> Parse(string input)
		{
			return from Match match in _regex.Matches(input) select ProcessMatch(match);
		}

		protected abstract T ProcessMatch(Match match);

		protected string StripTags(string input)
		{
			return Regex.Replace(input, "<.*?>", string.Empty);
		}
	}

	public class HeadingFilter : BaseFilter<Heading>
	{
		public HeadingFilter() : base(@"<h([1-6]).*?>(.*?)</h\1>") { }

		protected override Heading ProcessMatch(Match match)
		{
			return new Heading
			{
				Level = int.Parse(match.Groups[0].Value),
				Text = StripTags(match.Groups[1].Value)
			};
		}
	}

	public class ImageFilter : BaseFilter<Image>
	{
		public ImageFilter() : base(@"<img ((src|alt|title|class|id)=([""'])?(.*?)\3 ?)+") { }

		protected override Image ProcessMatch(Match match)
		{
			var str = match.Groups[1].Value;

			return new Image
			{
				Alt = GetAttribute("alt", str),
				Title = GetAttribute("title", str),
				Url = GetAttribute("src", str)
			};
		}

		private static string GetAttribute(string name, string input)
		{
			return Regex.Match(input, name + "=([\"']?)(.*?)\\1", RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Value;
		}
	}

	public class AssetFilter : BaseFilter<Asset>
	{
		public AssetFilter() : base(@"<(script|link).*?(src|href)=([""'])?(.*?)\\1") { }

		protected override Asset ProcessMatch(Match match)
		{
			var typestr = match.Groups[0].Value.ToLower();

			AssetType type;

			switch (typestr)
			{
				case "script":
					type = AssetType.JavaScript;
					break;
				case "link":
					type = AssetType.Link;
					break;
				default:
					type = AssetType.Unknown;
					break;
			}

			return new Asset
			{
				Type = type,
				Url = match.Groups[3].Value
			};
		}
	}

	public interface IFilterFactory
	{
		IEnumerable<IFilter> CreateFilters();
	}

	public class FilterFactory : IFilterFactory
	{
		private readonly HashSet<IFilter> _filters = new HashSet<IFilter>();

		public IEnumerable<IFilter> CreateFilters()
		{
			if (_filters.Any())
			{
				return _filters;
			}

			return new List<IFilter>
			{
				new HeadingFilter(), 
				new ImageFilter(), 
				new AssetFilter()
			};
		}

		public void AddFilter(IFilter filter)
		{
			_filters.Add(filter);
		}

		public void Reset()
		{
			_filters.Clear();
		}
	}

	// DOMAIN CONCEPTS DON'T BELONG HERE


}