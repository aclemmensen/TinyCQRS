using System;

namespace TinyCQRS.ReadModel.Interfaces
{
    public interface IDto
    {
		Guid Id { get; }
    }

	public abstract class Dto : IDto
	{
		public Guid Id { get; set; }
	}

	public abstract class ValueObject
	{
		
	}
}