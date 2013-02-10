using System;

namespace TinyCQRS.Contracts
{
	public abstract class Command : Message
	{
		protected Command(Guid aggregateId) : base(aggregateId) { }
	}
}