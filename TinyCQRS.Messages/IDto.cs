using System;

namespace TinyCQRS.Contracts
{
	public interface IReadModel
	{
		object Id { get; set; }
	}

	public abstract class ReadModelBase : IReadModel
	{
		public object Id { get; set; }
	}

	public abstract class Dto : ReadModelBase
	{
		public long LocalId { get; set; }
	}

	public abstract class Entity : ReadModelBase
	{
		public Guid GlobalId { get; set; }
	}

	public abstract class ValueObject
	{
		
	}
}