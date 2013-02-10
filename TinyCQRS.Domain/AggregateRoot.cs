using System;
using System.Collections;
using System.Collections.Generic;
using ReflectionMagic;
using TinyCQRS.Contracts;

namespace TinyCQRS.Domain
{
	public interface IEventSourced
	{
		IEnumerable<Event> PendingEvents { get; }
		Guid Id { get; }
		int Version { get; set; }

		void ClearPendingEvents();
		void LoadFrom(IEnumerable<Event> history);
	}

	public interface ISaga : IEventSourced
	{
		ICollection<Command> PendingMessages { get; }

		void ClearPendingMessages();
	}

	public abstract class EventSourced : IEventSourced
	{
		protected Guid _id;

		private readonly List<Event> _pendingEvents = new List<Event>();
		public IEnumerable<Event> PendingEvents { get { return _pendingEvents; } }

		public Guid Id { get { return _id; } }
		public int Version { get; set; }

		public void ClearPendingEvents()
		{
			_pendingEvents.Clear();
		}

		public void LoadFrom(IEnumerable<Event> history)
		{
			foreach (var e in history) ApplyChange(e, false);
		}

		protected void ApplyChange(Event message)
		{
			ApplyChange(message, true);
		}

		protected void ApplyChange(Event message, bool isNew)
		{
			this.AsDynamic().Apply(message);

			if (isNew) _pendingEvents.Add(message);
			else Version = message.Version;
		}
	}

	public abstract class AggregateRoot : EventSourced
	{
	}


	public abstract class Saga : EventSourced, ISaga
	{
		private readonly List<Command> _pendingMessages = new List<Command>();

		public ICollection<Command> PendingMessages { get { return _pendingMessages; }}

		public void ClearPendingMessages()
		{
			_pendingMessages.Clear();
		}

		protected void Dispatch(Command command)
		{
			_pendingMessages.Add(command);
		}

		public void Transition(Event @event)
		{
			ApplyChange(@event, true);
		}
	}
}