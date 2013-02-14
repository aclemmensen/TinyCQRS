using System;
using System.Collections.Generic;

namespace TinyCQRS.Contracts.Commands
{
	public class CreateNewSite : Command
	{
		public string Name { get; private set; }
		public string Root { get; private set; }

		public CreateNewSite(Guid siteId, string name, string root) : base(siteId)
		{
			Name = name;
			Root = root;
		}
	}

	public class ValidatePage : Command
	{
		public Guid PageId { get; set; }
		public string NewContent { get; set; }

		public ValidatePage(Guid crawlId, Guid pageId, string newContent)
			: base(crawlId)
		{
			PageId = pageId;
			NewContent = newContent;
		}
	}

	public class ProcessPages : Command
	{
		public IEnumerable<Guid> PageIds { get; set; }

		public ProcessPages(Guid siteId, IEnumerable<Guid> pageIds) : base(siteId)
		{
			PageIds = pageIds;
		}
	}

	public class SpellcheckPages : Command
	{
		public IEnumerable<Guid> PageIds { get; set; }

		public SpellcheckPages(Guid siteId, IEnumerable<Guid> pageIds) : base(siteId)
		{
			PageIds = pageIds;
		}
	}

	public class CreateSpellcheckConfiguration : Command
	{
		public string PrimaryLanguageKey { get; set; }
		public string SecondaryLanguageKey { get; set; }
		public IEnumerable<string> Including { get; set; }
		public IEnumerable<string> Excluding { get; set; }

		public CreateSpellcheckConfiguration(Guid siteId, string primaryLanguageKey, string secondaryLanguageKey, IEnumerable<string> including, IEnumerable<string> excluding) : base(siteId)
		{
			PrimaryLanguageKey = primaryLanguageKey;
			SecondaryLanguageKey = secondaryLanguageKey;
			Including = including;
			Excluding = excluding;
		}
	}
}