using System;
using Castle.Windsor;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Messages;

namespace TinyCQRS.Client
{
	public class CommandDispatcher : ICommandDispatcher
	{
		private readonly IWindsorContainer _container;
		private readonly ILogger _logger;

		public CommandDispatcher(IWindsorContainer container, ILogger logger)
		{
			_container = container;
			_logger = logger;
		}

		public void Dispatch<T>(T command) where T : Command
		{
			var handler = _container.Resolve<IHandle<T>>();
			
			if (handler == null)
			{
				throw new ApplicationException("No handler found for " + typeof (T).Name);
			}

			_logger.Log(string.Format("{0} handling {1}", handler.GetType().FullName, typeof(T).Name));

			handler.Handle(command);
		}
	}
}