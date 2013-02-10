using System;
using System.Collections.Generic;
using System.Linq;
using ReflectionMagic;
using TinyCQRS.Contracts;

namespace TinyCQRS.Infrastructure.Persistence
{
	public class InMemoryMessageBus : IMessageBus, IDisposable
    {
		private readonly IResolver _resolver;
		private readonly ILogger _logger;
		private readonly Dictionary<Type, List<IConsume>> _subscribers = new Dictionary<Type, List<IConsume>>();
		private IRelease<IEnumerable<IConsume>> _consumers;
		private bool _initialized;

		public InMemoryMessageBus(IResolver resolver, ILogger logger)
		{
			_resolver = resolver;
			_logger = logger;
		}

		private void Initialize()
		{
			if (_initialized) return;

			_consumers = _resolver.ResolveAll<IConsume>();
			Subscribe(_consumers.Instance.ToArray());

			_initialized = true;
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
			// Do lazy initialization to avoid problems with circular dependencies.
			// This is definitely not pretty, but it'll ensure the entire dependency
			// chain can be resolved before we hook up event consumers.
			if (!_initialized)
			{
				Initialize();
			}

            foreach (var subscriber in _subscribers.Where(x => x.Key == @event.GetType()).SelectMany(x => x.Value))
            {
				_logger.Log("Consumer {0} processing {1}", subscriber.GetType().Name, @event.GetType().Name);
                subscriber.AsDynamic().Process(@event);
            }
        }

		public void Dispose()
		{
			_consumers.Dispose();
		}
    }
}