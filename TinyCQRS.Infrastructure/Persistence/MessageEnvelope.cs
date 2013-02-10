using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using TinyCQRS.Contracts;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class MessageEnvelope
	{
		[Key]
		public long Id { get; set; }
		
		public Guid AggregateId { get; set; }

		public Guid MessageId { get; set; }
		public Guid? CorrelationId { get; set; }
		public Guid? SessionId { get; set; }
		
		public int Version { get; set; }
		public DateTime Created { get; set; }

		public string Data { get; set; }
		public string Type { get; set; }

		private static readonly EventContractResolver _resolver = new EventContractResolver();

		public Event Event
		{
			get
			{
				var res = (Event)JsonConvert.DeserializeObject(Data, new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.All,
					ContractResolver = _resolver
				});

				res.CorrelationId = CorrelationId ?? Guid.Empty;
				res.AggregateId = AggregateId;
				res.Version = Version;

				return res;
			}
			set
			{
				var x = value;
				
				Data = JsonConvert.SerializeObject(x, new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.All,
					ContractResolver = _resolver
				});
				
				Type = x.GetType().Name;
			}
		}

		public MessageEnvelope()
		{
			Created = DateTime.UtcNow;
		}

		public MessageEnvelope(Event @event) : this()
		{
			CorrelationId = @event.CorrelationId;
			AggregateId = @event.AggregateId;
			Version = @event.Version;
			Event = @event;
		}

		private class EventContractResolver : DefaultContractResolver
		{
			private static readonly List<Type> SkipTypes = new List<Type> {typeof(Event), typeof(Message), typeof(Command)} ;

			protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, MemberSerialization memberSerialization)
			{
				if (!SkipTypes.Contains(member.DeclaringType))
				{
					return base.CreateProperty(member, memberSerialization);
				}
				
				return null;
			}
		}
	}
}