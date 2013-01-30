using System;

namespace TinyCQRS.Domain.DomainModel
{
	public abstract class Entity
	{
		public Guid Id { get; protected set; }
	}
}