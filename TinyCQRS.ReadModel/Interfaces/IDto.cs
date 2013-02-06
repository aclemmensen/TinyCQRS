using System;

namespace TinyCQRS.ReadModel.Interfaces
{
	public abstract class Dto
	{
		public long Id { get; set; }
	}

	public abstract class Entity
	{
		public Guid Id { get; set; }
	}

	public abstract class ValueObject
	{
		
	}
}