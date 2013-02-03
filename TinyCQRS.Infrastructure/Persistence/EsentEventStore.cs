using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Isam.Esent.Collections.Generic;
using Newtonsoft.Json;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages;

namespace TinyCQRS.Infrastructure.Persistence
{
    public class EsentEventStore : IEventStore, IDisposable
    {
        private readonly string _foldername;
        private readonly Dictionary<Guid, PersistentDictionary<int,string>> _storage;
        private bool _disposed;
		
		public int Processed { get; private set; }

        public EsentEventStore(string foldername)
        {
            _foldername = foldername;
            _storage = new Dictionary<Guid, PersistentDictionary<int, string>>();
        }


	    public IEnumerable<Event> GetEventsFor(Guid id)
        {
            DisposeGuard();

            PersistentDictionary<int,string> events;

            if (!_storage.TryGetValue(id, out events))
            {
                events = CreateFor(id);
            }

            var data = events
                .OrderBy(x => x.Key)
                .Select(x => Deserialize<Event>(x.Value));

            return data;
        }

		public Event GetLastEventFor(Guid id)
		{
			return GetEventsFor(id).LastOrDefault();
		}

        public void StoreEvent(Event @event)
        {
            DisposeGuard();
			Processed++;

            PersistentDictionary<int, string> events;

            if (!_storage.TryGetValue(@event.AggregateId, out events))
            {
				events = CreateFor(@event.AggregateId);
            }

            events[@event.Version] = Serialize(@event);
            events.Flush();
        }

        private PersistentDictionary<int,string> CreateFor(Guid id)
        {
            _storage[id] = new PersistentDictionary<int, string>(_foldername + "/agg_" + id.ToString());
            return _storage[id];
        }

        private static string Serialize(Event @event)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            return JsonConvert.SerializeObject(@event, settings);
        }

        private static Event Deserialize<T>(string serialized) where T : Event
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var o = JsonConvert.DeserializeObject(serialized, settings);
            
            return (T) o;
        }

        private void DisposeGuard()
        {
            if (_disposed) throw new ApplicationException("This event store has been disposed.");
        }

        public void Dispose()
        {
            DisposeGuard();
            
            foreach (var storage in _storage.Values)
            {
                storage.Dispose();
            }
            _disposed = true;
        }
    }
}