using System;

namespace TinyCQRS.Contracts
{
	public abstract class Event : Message
	{
		public int Version { get; set; }

		protected Event(Guid aggregateId) : base(aggregateId) { }
	}
}