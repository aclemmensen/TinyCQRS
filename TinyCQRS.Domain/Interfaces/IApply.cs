﻿using TinyCQRS.Messages;

namespace TinyCQRS.Domain.Interfaces
{
	public interface IApply<T> where T : Event
	{
		void Apply(T @event);
	}
}