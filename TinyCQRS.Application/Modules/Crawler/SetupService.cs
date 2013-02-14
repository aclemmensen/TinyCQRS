using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Services;
using TinyCQRS.Infrastructure;

namespace TinyCQRS.Application.Modules.Crawler
{
	public class SetupService : ISetupService
	{
		private readonly ICommandDispatcher _dispatcher;

		public SetupService(ICommandDispatcher dispatcher)
		{
			_dispatcher = dispatcher;
		}

		public void CreateNewSite(CreateNewSite command)
		{
			_dispatcher.Dispatch(command);
		}

		public void CreateSpellcheckConfiguration(CreateSpellcheckConfiguration command)
		{
			_dispatcher.Dispatch(command);
		}
	}
}