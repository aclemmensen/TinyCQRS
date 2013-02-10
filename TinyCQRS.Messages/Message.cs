using System;

namespace TinyCQRS.Contracts
{
    public abstract class Message
    {
		public Guid AggregateId { get; set; }
		public Guid CorrelationId { get; set; }

        protected Message() { }

        protected Message(Guid aggregateId) : this()
        {
            AggregateId = aggregateId;
        }
    }
}
