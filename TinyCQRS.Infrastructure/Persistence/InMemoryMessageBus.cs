using System;
using System.Collections.Generic;
using System.Linq;
using ReflectionMagic;
using TinyCQRS.Contracts;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class InMemoryMessageBus : IMessageBus
    {
	    private readonly IResolver _resolver;
	    private readonly List<Event> _events = new List<Event>();
		private readonly Dictionary<Type, List<IConsume>> _subscribers = new Dictionary<Type, List<IConsume>>();

		public InMemoryMessageBus(IResolver resolver)
		{
			_resolver = resolver;
			var consumers = resolver.ResolveAll<IConsume>().ToArray();
			Subscribe(consumers);
		}

	    public void Subscribe(params IConsume[] subscribers)
        {
			foreach (var subscriber in subscribers)
			{
				var types = subscriber.GetType()
					.GetInterfaces()
					.Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IConsume<>));

				foreach (var type in types.Select(x => x.GenericTypeArguments[0]))
				{
					if (!_subscribers.ContainsKey(type))
					{
						_subscribers[type] = new List<IConsume>();
					}

					_subscribers[type].Add(subscriber);
				}
			}
        }

        public void Notify(Event @event)
        {
            foreach (var subscriber in _subscribers.Where(x => x.Key == @event.GetType()).SelectMany(x => x.Value))
            {
                subscriber.AsDynamic().Process(@event);
            }
        }
    }
}