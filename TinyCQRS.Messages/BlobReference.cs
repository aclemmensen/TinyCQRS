using System;

namespace TinyCQRS.Contracts
{
	public class BlobReference
	{
		public Guid AggregateId { get; set; }
		public Guid ItemId { get; set; }

		public BlobReference() { }

		public BlobReference(Guid aggregateId, Guid itemId)
		{
			AggregateId = aggregateId;
			ItemId = itemId;
		}

		public override string ToString()
		{
			return BlobReference<object>.CreateId(AggregateId, ItemId);
		}
	}

	public class BlobReference<T> : BlobReference
	{
		public T Payload { get; set; }

		public BlobReference() { }

		public BlobReference(Guid aggregateId, Guid itemId)
		{
			AggregateId = aggregateId;
			ItemId = itemId;
		}

		public BlobReference(Guid aggregateId, Guid itemId, T payload) : this(aggregateId, itemId)
		{
			Payload = payload;
		}

		public override string ToString()
		{
			return CreateId(AggregateId, ItemId);
		}

		public static string CreateId(Guid aggregateId, Guid itemId)
		{
			return string.Format("blob_{0}_{1}", aggregateId, itemId);
		}
	}
}