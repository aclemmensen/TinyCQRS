using System;
using TinyCQRS.Contracts;
using TinyCQRS.Domain.Interfaces;

namespace TinyCQRS.Infrastructure
{
	public class CommandDispatcher : ICommandDispatcher
	{
		private readonly IResolver _resolver;
		private readonly ILogger _logger;

		public CommandDispatcher(IResolver resolver, ILogger logger)
		{
			_resolver = resolver;
			_logger = logger;
		}

		public void Dispatch<T>(T command) where T : Command
		{
			var handler = _resolver.Resolve<IHandle<T>>();

			if (handler == null)
			{
				Dispatch(command as Command);
				return;
			}

			_logger.Log(string.Format("{0} handling {1}", handler.GetType().Name, typeof(T).Name));

			handler.Handle(command);
		}

		public void Dispatch(Command command)
		{
			var handlerType = typeof(IHandle<>).MakeGenericType(command.GetType());
			var handler = _resolver.Resolve(handlerType);

			if (handler == null)
			{
				throw new ApplicationException("No handler found for " + command.GetType().Name);
			}

			var method = handler.GetType().GetMethod("Handle", new[] {command.GetType()});
			method.Invoke(handler, new object[] {command});
		}
	}
}