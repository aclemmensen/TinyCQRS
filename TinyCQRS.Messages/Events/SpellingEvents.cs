using System;
using System.Collections.Generic;

namespace TinyCQRS.Contracts.Events
{
	public class SpellingConfigurationCreated : Event
	{
		public string PrimaryLanguageKey { get; set; }
		public string SecondaryLanguageKey { get; set; }
		public bool IsDefault { get; set; }
		public IEnumerable<string> Including { get; set; }
		public IEnumerable<string> Excluding { get; set; }

		public SpellingConfigurationCreated(Guid siteId, string primaryKey, string secondaryKey, bool isDefault, IEnumerable<string> including, IEnumerable<string> excluding) : base(siteId)
		{
			PrimaryLanguageKey = primaryKey;
			SecondaryLanguageKey = secondaryKey;
			IsDefault = isDefault;
			Including = including;
			Excluding = excluding;
		}
	}

	public class PageSpellcheckCompleted : Event
	{
		public Guid PageId { get; set; }
		public IEnumerable<string> ConfirmedMisspellings { get; set; }
		public IEnumerable<string> PotentialMisspellings { get; set; }

		public PageSpellcheckCompleted(Guid siteId, Guid pageId, IEnumerable<string> confirmedMisspellings, IEnumerable<string> potentialMisspellings) : base(siteId)
		{
			PageId = pageId;
			ConfirmedMisspellings = confirmedMisspellings;
			PotentialMisspellings = potentialMisspellings;
		}
	}
}