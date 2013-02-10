using System;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Persistence
{
    public class EventedRepository<T> : IRepository<T> where T : IEventSourced, new()
    {
        private readonly IEventStore _eventStore;
	    private readonly IMessageBus _bus;

	    public EventedRepository(IEventStore eventStore, IMessageBus bus)
        {
	        _eventStore = eventStore;
		    _bus = bus;
        }

	    public T GetById(Guid id)
	    {
			var obj = new T();
			obj.LoadFrom(_eventStore.GetEventsFor(id));
			return obj;
	    }

        public void Save(T aggregate, int? expectedVersion = null)
        {
            int exp = expectedVersion ?? aggregate.Version;

	        var lastEvent = _eventStore.GetLastEventFor(aggregate.Id);
            if (lastEvent != null && exp != 0 && lastEvent.Version != exp)
            {
                throw new ConcurrencyException(string.Format(
                    "Expected aggregate version {0}, got version {1}",
                    exp, 
                    lastEvent.Version));
            }

            var i = (lastEvent == null) ? 0 : lastEvent.Version;
            foreach (var e in aggregate.PendingEvents)
            {
                e.Version = ++i;
                aggregate.Version = e.Version;
                _eventStore.StoreEvent<T>(e);
				_bus.Notify(e);
            }

            aggregate.ClearPendingEvents();
        }
    }

	public class SagaRepository<T> : ISagaRepository<T> where T : ISaga, new()
	{
		private readonly IRepository<T> _repository;
		private readonly ICommandDispatcher _dispatcher;

		public SagaRepository(IRepository<T> repository, ICommandDispatcher dispatcher)
		{
			_repository = repository;
			_dispatcher = dispatcher;
		}

		public T GetById(Guid id)
		{
			return _repository.GetById(id);
		}

		public void Save(T aggregate, int? expectedVersion = null)
		{
			_repository.Save(aggregate, expectedVersion);

			foreach (var message in aggregate.PendingMessages)
			{
				_dispatcher.Dispatch(message);
			}

			aggregate.ClearPendingMessages();
		}
	}
}