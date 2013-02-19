using System;
using System.Collections.Generic;
using TinyCQRS.Contracts;
using TinyCQRS.Domain;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class EventedRepository<T> : IRepository<T> where T : IEventSourced, new()
    {
        private readonly IEventStore _eventStore;
	    private readonly ICommandDispatcher _dispatcher;

	    public EventedRepository(IEventStore eventStore, ICommandDispatcher dispatcher)
        {
	        _eventStore = eventStore;
		    _dispatcher = dispatcher;
        }

	    public T GetById(Guid id)
	    {
			var obj = new T();
			obj.LoadFrom(_eventStore.GetEventsFor<T>(id));
			return obj;
	    }

        public void Save(T aggregate, int? expectedVersion = null)
        {
            var exp = expectedVersion ?? aggregate.Version;

	        var lastVersion = _eventStore.GetVersionFor<T>(aggregate.Id);
			if (lastVersion != 0 && exp != 0 && lastVersion != exp)
            {
                throw new ConcurrencyException(string.Format(
                    "Expected aggregate version {0}, got version {1}",
                    exp, 
                    lastVersion));
            }

	        var i = lastVersion;
            foreach (var e in aggregate.PendingEvents)
            {
                e.Version = ++i;
                aggregate.Version = e.Version;
                _eventStore.StoreEvent<T>(e);
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