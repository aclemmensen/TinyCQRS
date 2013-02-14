using System.Collections.Generic;

namespace TinyCQRS.Domain.Blobs
{
	public class PageProfile : ValueObject
	{
		public string Url { get; set; }
		public PageHash Hash { get; set; }

		public string RawContent { get; set; }
		public string Text { get; set; }

		public Dictionary<string, string> ResponseHeaders { get; set; }

		public PageProfile()
		{
			Hash = new PageHash();
			ResponseHeaders = new Dictionary<string, string>();
		}

		public PageProfile(string url, string html, string stripped)
		{
			RawContent = html;
			Text = stripped;
			Url = url;
			Hash = PageHash.Create(html, stripped);
		}

		public class PageHash : ValueObject
		{
			public string RawHash { get; set; }
			public string TextHash { get; set; }

			public static PageHash Create(string raw, string stripped)
			{
				return new PageHash
				{
					RawHash = HashingHelper.Hash(raw),
					TextHash = HashingHelper.Hash(stripped)
				};
			}
		}

	}
}