using System;
using System.Collections.Generic;
using TinyCQRS.Contracts;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Infrastructure.Caching;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class CachingRepository<T> : IRepository<T> where T : IEventSourced, new()
	{
		private readonly IRepository<T> _innerRepository;
		private readonly ICache<T> _cache;

		public CachingRepository(IRepository<T> innerRepository, ICache<T> cache)
		{
			_innerRepository = innerRepository;
			_cache = cache;
		}

		public T GetById(Guid id)
		{
			return _cache.Get(id, () => _innerRepository.GetById(id));
		}

		public void Save(T aggregate, int? expectedVersion = null)
		{
			_innerRepository.Save(aggregate, expectedVersion);

			_cache.Set(aggregate.Id, aggregate);
		}
	}

    public class EventedRepository<T> : IRepository<T> where T : IEventSourced, new()
    {
        private readonly IEventStore _eventStore;
	    private readonly IMessageBus _bus;
	    private readonly ICommandDispatcher _dispatcher;

	    public EventedRepository(IEventStore eventStore, IMessageBus bus, ICommandDispatcher dispatcher)
        {
	        _eventStore = eventStore;
		    _bus = bus;
		    _dispatcher = dispatcher;
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

	        var saga = aggregate as ISaga;
			if (saga != null)
			{
				// It is necessary to put all commands into a new collection and clear the original,
				// otherwise we might end up looping indefinitely in some cases when caching aggregates
				// in memory. This is not pretty, but neither is inifinite recursion.
				var messages = new HashSet<Command>(saga.PendingMessages);
				saga.ClearPendingMessages();

				foreach (var message in messages)
				{
					_dispatcher.Dispatch(message);
				}
			}
        }
    }
}