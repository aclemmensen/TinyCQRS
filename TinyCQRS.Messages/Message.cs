using System;

namespace TinyCQRS.Messages
{
    public abstract class Message
    {
        public Guid AggregateId { get; set; }

        protected Message()
        {
            AggregateId = Guid.Empty;
        }

        protected Message(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }
    }

    public abstract class Event : Message
    {
        public int Version { get; set; }

        public Guid CorrelationId { get; set; }

        protected Event(Guid aggregateId) : base(aggregateId) { }
    }

    public abstract class Command : Message
    {
        protected Command(Guid aggregateId) : base(aggregateId) { }
    }
}
