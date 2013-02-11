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

	public class BlobReference
	{
		public Guid AggregateId { get; set; }
		public Guid ItemId { get; set; }

		public BlobReference(Guid aggregateId)
		{
			AggregateId = aggregateId;
		}

		public BlobReference(Guid aggregateId, Guid itemId) : this(aggregateId)
		{
			ItemId = itemId;
		}

		public override string ToString()
		{
			return string.Format("{0}_{1}", AggregateId, ItemId);
		}
	}

	public class BlobReference<T> : BlobReference
	{
		public T Payload { get; set; }

		public BlobReference(Guid aggregateId, T payload) : base(aggregateId)
		{
			Payload = payload;
		}

		public BlobReference(Guid aggregateId, Guid itemId, T payload) : base(aggregateId, itemId)
		{
			Payload = payload;
		}
	}
}
