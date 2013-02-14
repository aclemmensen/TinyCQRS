using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TinyCQRS.Contracts;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Events;
using TinyCQRS.Contracts.Models;
using TinyCQRS.Domain.Blobs;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Domain.Services;

namespace TinyCQRS.Domain.Models.QualityAssurance
{
	public class Site : Saga,
		IApply<SiteCreatedEvent>,
		IApply<NewPagesAdded>,
		IApply<ExistingPagesUpdated>,
		IApply<ExistingPagesRemoved>,
		IApply<SpellingConfigurationCreated>,
		IApply<PageSpellcheckCompleted>,
		IApply<PageComponentsIdentified>
	{
		private string _root;
		private string _name;
		private readonly HashSet<Guid> _pages = new HashSet<Guid>();
		private readonly SpellcheckConfigurations _spellingConfigurations = new SpellcheckConfigurations();

		public Site() { }

		public Site(Guid id, string name, string root)
		{
			ApplyChange(new SiteCreatedEvent(id, name, root));
		}

		public void AddPages(IEnumerable<Guid> pageIds, DateTime time)
		{
			var newPages = pageIds as IList<Guid> ?? pageIds.ToList();
			if (newPages.Any(_pages.Contains))
			{
				throw new InvalidOperationException("Cannot add existing page to site");
			}

			if (!newPages.Any())
			{
				return;
			}

			ApplyChange(new NewPagesAdded(_id, newPages, time));
			
			Dispatch(new ProcessPages(_id, newPages));
		}

		public void UpdatePages(IEnumerable<Guid> pageIds, DateTime time)
		{
			var updatedPages = pageIds as IList<Guid> ?? pageIds.ToList();
			if (updatedPages.Any(x => !_pages.Contains(x)))
			{
				throw new InvalidOperationException("Cannot update a page that hasn't been added before.");
			}
			
			if (!updatedPages.Any())
			{
				return;
			}

			ApplyChange(new ExistingPagesUpdated(_id, updatedPages, time));
			
			Dispatch(new ProcessPages(_id, updatedPages));
		}

		public void RemovePages(IEnumerable<Guid> pageIds, DateTime time)
		{
			var removedPages = pageIds as IList<Guid> ?? pageIds.ToList();
			if (removedPages.Any(x => !_pages.Contains(x)))
			{
				throw new InvalidOperationException("Cannot remove a page that doesn't exist on the site.");
			}

			if (!removedPages.Any())
			{
				return;
			}

			ApplyChange(new ExistingPagesRemoved(_id, removedPages, time));
		}

		public void AddNewSpellingConfiguration(string primaryKey, string secondaryKey, IEnumerable<string> including, IEnumerable<string> excluding)
		{
			if (string.IsNullOrEmpty(primaryKey))
			{
				throw new ArgumentNullException("primaryKey");
			}

			var isDefault = !including.Any() && !excluding.Any();
			
			ApplyChange(new SpellingConfigurationCreated(_id, primaryKey, secondaryKey, isDefault, including, excluding));
		}

		public void ProcessPageText(IEnumerable<Guid> pageIds, TextProcessingService textProcessing, IBlobStorage blobs)
		{
			var toSpellcheck = new List<Guid>();

			foreach (var pageId in pageIds)
			{
				var blob = new BlobReference(_id, pageId);
				var page = blobs.Get<PageProfile>(blob);

				var components = new PageComponents
				{
					Assets = textProcessing.Get<Asset>(page.RawContent),
					Headings = textProcessing.Get<Heading>(page.RawContent),
					Images = textProcessing.Get<Image>(page.RawContent),
					TextContent = textProcessing.GetPlainText(page.RawContent)
				};

				if (components.TextContent != null)
				{
					page.Text = components.TextContent;
					blobs.Save(blob, page);	

					toSpellcheck.Add(pageId);
				}

				ApplyChange(new PageComponentsIdentified(_id, pageId, components));
			}

			Dispatch(new SpellcheckPages(_id, toSpellcheck));
		}

		public void SpellcheckPages(IEnumerable<Guid> pageIds, SpellcheckService service)
		{
			if (!_spellingConfigurations.Configurations.Any())
			{
				throw new InvalidOperationException("No spellchecking configurations created for this site.");
			}

			var results = service.CheckPages(_id, pageIds, _spellingConfigurations);
			
			foreach (var result in results)
			{
				ApplyChange(new PageSpellcheckCompleted(_id, result.PageId, result.ConfirmedMisspellings, result.PotentialMisspellings));
			}
		}

		// APPLY //

		public void Apply(SpellingConfigurationCreated @event)
		{
			var configuration = new SpellcheckConfiguration
			{
				Includes = new HashSet<string>(@event.Including),
				Excludes = new HashSet<string>(@event.Excluding),
				IsDefault = @event.IsDefault,
				PrimaryLanguageKey = @event.PrimaryLanguageKey,
				SecondaryLanguageKey = @event.SecondaryLanguageKey
			};

			_spellingConfigurations.Add(configuration);
		}

		public void Apply(PageSpellcheckCompleted @event)
		{}

		public void Apply(SiteCreatedEvent @event)
		{
			_id = @event.AggregateId;
			_name = @event.Name;
			_root = @event.Root;
		}

		public void Apply(NewPagesAdded @event)
		{
			foreach (var id in @event.AddedPages)
			{
				_pages.Add(id);
			}
		}

		public void Apply(ExistingPagesUpdated @event)
		{}

		public void Apply(ExistingPagesRemoved @event)
		{
			foreach (var id in @event.RemovedPages)
			{
				_pages.Remove(id);
			}
		}

		public void Apply(PageComponentsIdentified @event)
		{}

		// ENTITIES //

		public class SpellcheckConfigurations
		{
			public HashSet<SpellcheckConfiguration> Configurations { get; private set; }

			public SpellcheckConfigurations()
			{
				Configurations = new HashSet<SpellcheckConfiguration>();
			}

			public SpellcheckConfiguration GetFor(string url)
			{
				return Configurations.FirstOrDefault(x => x.Matches(url)) ?? Configurations.FirstOrDefault(x => x.IsDefault);
			}

			public void Add(SpellcheckConfiguration configuration)
			{
				Configurations.Add(configuration);
			}
		}

		public class SpellcheckConfiguration : ValueObject
		{
			public Guid Id { get; private set; }

			public bool IsDefault { get; set; }
			public string PrimaryLanguageKey { get; set; }
			public string SecondaryLanguageKey { get; set; }

			public HashSet<string> Includes { get; set; }
			public HashSet<string> Excludes { get; set; }

			public ICollection<Decision> Decisions { get; set; }

			public SpellcheckConfiguration()
			{
				Decisions = new Collection<Decision>();
				Includes = new HashSet<string>();
				Excludes = new HashSet<string>();
			}

			public bool Matches(string url)
			{
				return Includes.Contains(url) && !Excludes.Contains(url);
			}
		}

		public class SpellcheckResult : ValueObject
		{
			public Guid SiteId { get; set; }
			public Guid PageId { get; set; }

			public IEnumerable<string> PotentialMisspellings { get; set; }
			public IEnumerable<string> ConfirmedMisspellings { get; set; }
		}

		public enum DecisionLevel
		{
			Page,
			Account,
			Global
		}

		public enum DecisionType
		{
			Ignore,
			Accept
		}

		public class Decision : ValueObject
		{
			public string Word { get; private set; }
			public DecisionLevel Level { get; private set; }
			public DecisionType Type { get; private set; }

			public Decision(string word, DecisionType type, DecisionLevel level = DecisionLevel.Account)
			{
				Word = word;
				Level = level;
				Type = type;
			}
		}

	}
}