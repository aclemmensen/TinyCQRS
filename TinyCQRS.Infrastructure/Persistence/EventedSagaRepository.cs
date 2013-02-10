using System;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class EventedSagaRepository<T> : ISagaRepository<T> where T : ISaga, new()
	{
		private readonly IEventStore<T> _eventStore;
		private readonly IMessageBus _bus;
		private readonly ICommandDispatcher _dispatcher;

		public EventedSagaRepository(IEventStore<T> eventStore, ICommandDispatcher dispatcher)
		{
			_eventStore = eventStore;
			_dispatcher = dispatcher;
		}

		public T GetById(Guid id)
		{
			var obj = new T();
			obj.LoadFrom(_eventStore.GetEventsFor(id));
			return obj;
		}

		public void Save(T saga, int? expectedVersion = null)
		{
			var exp = expectedVersion ?? saga.Version;

			var lastEvent = _eventStore.GetLastEventFor(saga.Id);
			if (lastEvent != null && exp != 0 && lastEvent.Version != exp)
			{
				throw new ConcurrencyException(string.Format(
					"Expected saga version {0}, got version {1}",
					exp,
					lastEvent.Version));
			}

			var i = (lastEvent == null) ? 0 : lastEvent.Version;
			foreach (var e in saga.PendingEvents)
			{
				e.Version = ++i;
				saga.Version = e.Version;
				_eventStore.StoreEvent(e);
			}

			foreach (var c in saga.PendingMessages)
			{
				_dispatcher.Dispatch(c);
			}

			saga.ClearPendingEvents();
			saga.ClearPendingMessages();
		}
	}
}