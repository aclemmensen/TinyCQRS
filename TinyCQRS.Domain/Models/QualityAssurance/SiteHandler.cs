using TinyCQRS.Contracts;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Events;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Domain.Services;

namespace TinyCQRS.Domain.Models.QualityAssurance
{
	public class SiteHandler :
		IHandle<CreateNewSite>,
		IHandle<CreateSpellcheckConfiguration>,
		IHandle<ProcessPages>,
		IHandle<SpellcheckPages>,
		IConsume<CrawlCompleted>
	{
		private readonly IRepository<Site> _sites;
		private readonly IBlobStorage _blobStorage;
		private readonly SpellcheckService _spellcheckService;
		private readonly TextProcessingService _textProcessing;

		public SiteHandler(IRepository<Site> sites, IBlobStorage blobStorage, SpellcheckService spellcheckService, TextProcessingService textProcessing)
		{
			_sites = sites;
			_blobStorage = blobStorage;
			_spellcheckService = spellcheckService;
			_textProcessing = textProcessing;
		}

		public void Handle(CreateNewSite command)
		{
			var site = new Site(command.AggregateId, command.Name, command.Root);
			_sites.Save(site);
		}

		public void Handle(CreateSpellcheckConfiguration command)
		{
			var site = _sites.GetById(command.AggregateId);
			site.AddNewSpellingConfiguration(command.PrimaryLanguageKey, command.SecondaryLanguageKey, command.Including, command.Excluding);
			_sites.Save(site);
		}

		public void Handle(ProcessPages command)
		{
			var site = _sites.GetById(command.AggregateId);
			site.ProcessPageText(command.PageIds, _textProcessing, _blobStorage);
			_sites.Save(site);
		}

		public void Handle(SpellcheckPages command)
		{
			var site = _sites.GetById(command.AggregateId);
			site.SpellcheckPages(command.PageIds, _spellcheckService);
			_sites.Save(site);
		}

		public void Process(CrawlCompleted @event)
		{
			var site = _sites.GetById(@event.SiteId);

			site.AddPages(@event.NewPages, @event.TimeOfCompletion);
			site.UpdatePages(@event.ChangedPages, @event.TimeOfCompletion);
			site.RemovePages(@event.MissingPages, @event.TimeOfCompletion);

			_sites.Save(site);
		}

	}
}