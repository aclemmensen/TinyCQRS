using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TinyCQRS.Contracts;
using TinyCQRS.Domain.Blobs;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Domain.Models.QualityAssurance;

namespace TinyCQRS.Domain.Services
{
	
	public class SpellcheckService
	{
		private readonly IBlobStorage _blobs;
		private readonly ISpellcheckerFactory _spellcheckerFactory;

		public SpellcheckService(IBlobStorage blobs, ISpellcheckerFactory spellcheckerFactory)
		{
			_blobs = blobs;
			_spellcheckerFactory = spellcheckerFactory;
		}

		public IEnumerable<Site.SpellcheckResult> CheckPages(Guid siteId, IEnumerable<Guid> pageIds, Site.SpellcheckConfigurations configurations)
		{
			var results = new List<Site.SpellcheckResult>();

			foreach (var pageId in pageIds)
			{
				var page = _blobs.Get<PageProfile>(new BlobReference(siteId, pageId));
				var config = configurations.GetFor(page.Url);

				if (config == null) continue;

				var spellchecker = _spellcheckerFactory.CreateFor(config.PrimaryLanguageKey, config.SecondaryLanguageKey);
				var result = spellchecker.Check(page.Text);

				if (result.HasData)
				{
					results.Add(new Site.SpellcheckResult
					{
						PageId = pageId,
						ConfirmedMisspellings = result.Misspellings,
						PotentialMisspellings = result.PotentialMisspellings,
						SiteId = siteId
					});
				}
			}

			return results;
		}
	}

	public interface ISpellcheckerFactory
	{
		ISpellchecker CreateFor(string primaryLanguageKey, string secondaryLanguageKey);
	}

	public interface ISpellchecker
	{
		Spellcheck Check(string text);
	}

	public class Spellcheck
	{
		public ICollection<string> Misspellings { get; private set; }
		public ICollection<string> PotentialMisspellings { get; private set; }

		public bool HasData { get { return Misspellings.Any() || PotentialMisspellings.Any(); } }

		public Spellcheck()
		{
			Misspellings = new List<string>();
			PotentialMisspellings = new List<string>();
		}
	}
}