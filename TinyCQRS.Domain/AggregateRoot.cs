using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReflectionMagic;
using TinyCQRS.Contracts;
using TinyCQRS.Domain.Interfaces;

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

		private static readonly Dictionary<Type,Dictionary<Type,MethodInfo>> _methodCache 
			= new Dictionary<Type, Dictionary<Type, MethodInfo>>();

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

		protected void ApplyChange<T>(T message) where T : Event
		{
			ApplyChange(message, true);
		}

		protected void ApplyChange<T>(T message, bool isNew) where T : Event
		{
			//this.AsDynamic().Apply(message);

			var applier = this as IApply<T>;
			if (applier != null)
			{
				//throw new ApplicationException("This entity does not implement IApply<" + typeof(T).Name + ">");
				applier.Apply(message);
			}
			else
			{
				
				Get(GetType(), message.GetType()).Invoke(this, new object[] {message});

				//this.AsDynamic().Apply(message);
			}

			if (isNew) _pendingEvents.Add(message);
			else Version = message.Version;
		}

		private static MethodInfo Get(Type aggregateType, Type messageType)
		{
			if (!_methodCache.ContainsKey(aggregateType))
			{
				_methodCache[aggregateType] = aggregateType
					.GetMethods(BindingFlags.Instance | BindingFlags.Public)
					.Where(x => x.Name == "Apply" && x.ReturnType == typeof(void))
					.ToDictionary(x => x.GetParameters().Single().ParameterType, x => x);
			}

			MethodInfo method;
			if (!_methodCache[aggregateType].TryGetValue(messageType, out method))
			{
				throw new MissingMethodException(string.Format("No Apply method on aggregate type {0} for event type {1}", aggregateType.Name, messageType.Name));
			}

			return method;
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