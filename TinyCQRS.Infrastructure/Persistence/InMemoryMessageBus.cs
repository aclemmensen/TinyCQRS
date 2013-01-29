using System;
using System.Collections.Generic;
using System.Linq;
using ReflectionMagic;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages;

namespace TinyCQRS.Infrastructure.Persistence
{
    public class InMemoryMessageBus : IMessageBus
    {
        private readonly List<Event> _events = new List<Event>();
        private readonly Dictionary<Type, List<ISubscriber>> _subscribers = new Dictionary<Type, List<ISubscriber>>();

        public void Subscribe(ISubscriber subscriber)
        {
            var types = subscriber.GetType()
                .GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISubscribeTo<>));

            foreach (var type in types)
            {
                var eventType = type.GenericTypeArguments[0];
                if (!_subscribers.ContainsKey(eventType))
                {
                    _subscribers[eventType] = new List<ISubscriber>();
                }

                _subscribers[eventType].Add(subscriber);
            }
        }

        public void Notify(Event @event)
        {
            foreach (var subscriber in _subscribers.Where(x => x.Key == @event.GetType()).SelectMany(x => x.Value))
            {
                subscriber.AsDynamic().Consume(@event);
            }
        }
    }
}