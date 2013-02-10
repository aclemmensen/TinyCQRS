using System;
using System.Collections.Generic;
using Stateless;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Events;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Domain.Models.QualityAssurance
{
	public class CrawlSaga : Saga,
		IApply<CrawlOrdered>,
		IApply<CrawlStarted>,
		IApply<PageChecked>,
		IApply<PageCreated>,
		IApply<PageContentChanged>,
		IApply<CrawlCompleted>
	{
		private Guid _siteId;
		private DateTime? _timeOfOrder;
		private DateTime? _startTime;
		private DateTime? _completionTime;
		private string _crawlerName;
		private readonly StateMachine<State, Trigger> _state;
		private readonly HashSet<Guid> _pages = new HashSet<Guid>();

		private readonly CrawlStatus _status = new CrawlStatus();

		private enum State
		{
			None = 0,
			Ordered,
			InProgress,
			Completed
		}

		private enum Trigger
		{
			CrawlOrderReceived,
			CrawlStarted,
			CrawlMarkedComplete
		}

		public CrawlSaga()
		{
			_state = new StateMachine<State,Trigger>(State.None);

			_state.Configure(State.None).Permit(Trigger.CrawlOrderReceived, State.Ordered);
			_state.Configure(State.Ordered).Permit(Trigger.CrawlStarted, State.InProgress);
			_state.Configure(State.InProgress).Permit(Trigger.CrawlMarkedComplete, State.Completed);
		}

		public CrawlSaga(Guid crawlId, Guid siteId, DateTime timeOfOrder) : this()
		{
			ApplyChange(new CrawlOrdered(crawlId, siteId, timeOfOrder));
		}

		public void StartCrawl(string crawlerName, DateTime startTime)
		{
			Guard(Trigger.CrawlStarted, "This crawl has already been started.");
			ApplyChange(new CrawlStarted(_id, crawlerName, startTime));
		}

		public void AddNewPage(Guid pageId, string url, string content, DateTime timeOfCreation)
		{
			Guard(State.InProgress, "Cannot add pages to a crawl that isn't running");
			Guard(pageId, string.Format("Cannot add page id {0} as new; it has already been seen", pageId));

			ApplyChange(new PageCreated(_id, _siteId, pageId, url, content, timeOfCreation));
		}

		public void PageCheckedWithoutChange(Guid pageId, DateTime timeOfCheck)
		{
			Guard(State.InProgress, "Cannot register page check to a crawl that isn't running");
			
			ApplyChange(new PageChecked(_id, pageId, timeOfCheck));
		}

		public void UpdatePageContent(Guid pageId, string newContent, DateTime timeOfChange)
		{
			Guard(State.InProgress, "Cannot update page content for a crawl that isn't running");
			
			ApplyChange(new PageContentChanged(_id, pageId, newContent, timeOfChange));
		}

		public void MarkCompleted(DateTime timeOfCompletion, IEnumerable<Guid> missingPages)
		{
			Guard(Trigger.CrawlMarkedComplete, "Cannot complete a crawl that isn't running.");
			ApplyChange(new CrawlCompleted(_id, _siteId, timeOfCompletion, _status.TotalCount, _status.NewPages, _status.ChangedPages, _status.UnchangedPages, missingPages));
		}

		private void Guard(Trigger trigger, string message)
		{
			if (!_state.CanFire(trigger))
			{
				throw new InvalidOperationException(string.Format("Error transitioning {0} -> {1}: {2}", _state.State, trigger, message));
			}
		}

		private void Guard(State state, string message)
		{
			if (!_state.IsInState(state))
			{
				throw new InvalidOperationException(string.Format("In state {0}, expected {1}: {2}", _state.State, state, message));
			}
		}

		private void Guard(Guid pageId, string message)
		{
			if (_pages.Contains(pageId))
			{
				throw new InvalidOperationException(message);
			}
		}

		public void Apply(CrawlOrdered @event)
		{
			_id = @event.AggregateId;
			_siteId = @event.SiteId;
			_timeOfOrder = @event.TimeOfOrder;

			_state.Fire(Trigger.CrawlOrderReceived);
		}

		public void Apply(CrawlStarted @event)
		{
			_startTime = @event.StartTime;
			_crawlerName = @event.CrawlerName;
			
			_state.Fire(Trigger.CrawlStarted);
		}

		public void Apply(PageChecked @event)
		{
			_status.UnchangedPages.Add(@event.PageId);
			_pages.Add(@event.PageId);
		}

		public void Apply(PageCreated @event)
		{
			_status.NewPages.Add(@event.PageId);
			_pages.Add(@event.PageId);
		}

		public void Apply(PageContentChanged @event)
		{
			_status.ChangedPages.Add(@event.PageId);
			_pages.Add(@event.PageId);
		}

		public void Apply(CrawlCompleted @event)
		{
			_completionTime = @event.TimeOfCompletion;
		}

		private class CrawlStatus
		{
			public List<Guid> NewPages { get; private set; }
			public List<Guid> ChangedPages { get; private set; }
			public List<Guid> UnchangedPages { get; private set; }
			public int TotalCount
			{
				get { return NewPages.Count + ChangedPages.Count + UnchangedPages.Count; }
			}

			public CrawlStatus()
			{
				NewPages = new List<Guid>();
				ChangedPages = new List<Guid>();
				UnchangedPages = new List<Guid>();
			}
		}
	}
}