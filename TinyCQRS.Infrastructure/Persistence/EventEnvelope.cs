using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Serialization;
using TinyCQRS.Messages;
using Newtonsoft.Json;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class EventEnvelope
	{
		[Key]
		public long Id { get; set; }
		
		public Guid AggregateId { get; set; }
		
		public Guid CorrelationId { get; set; }
		
		public int Version { get; set; }

		public DateTime Created { get; set; }

		public string Data { get; set; }
		public string Type { get; set; }

		private static readonly EventContractResolver _resolver = new EventContractResolver();

		public Event Event
		{
			get
			{
				return As<Event>();
			}
			set
			{
				var x = value;
				Data = JsonConvert.SerializeObject(x, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ContractResolver = _resolver });
				Type = x.GetType().Name;
			}
		}

		public EventEnvelope()
		{
			Created = DateTime.UtcNow;
		}

		public EventEnvelope(Event @event) : this()
		{
			AggregateId = @event.AggregateId;
			CorrelationId = @event.CorrelationId;
			Version = @event.Version;
			Event = @event;
		}

		public T As<T>() where T : Event
		{
			var res = (T) JsonConvert.DeserializeObject(Data, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All, 
				ContractResolver = _resolver
			});

			res.AggregateId = AggregateId;
			res.CorrelationId = CorrelationId;
			res.Version = Version;

			return res;
		}

		private class EventContractResolver : DefaultContractResolver
		{
			protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, MemberSerialization memberSerialization)
			{
				if (member.DeclaringType != typeof (Event) && member.DeclaringType != typeof(Message))
				{
					return base.CreateProperty(member, memberSerialization);
				}
				
				return null;
			}
		}
	}
}