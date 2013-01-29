using System;
using System.Collections.Generic;
using ReflectionMagic;
using TinyCQRS.Messages;

namespace TinyCQRS.Domain
{
	public abstract class AggregateRoot
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

		public void ApplyChange(Event message)
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
}