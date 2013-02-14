using TinyCQRS.Contracts.Commands;

namespace TinyCQRS.Contracts.Services
{
	public interface ISetupService : IService
	{
		void CreateNewSite(CreateNewSite command);
		void CreateSpellcheckConfiguration(CreateSpellcheckConfiguration command);
	}
}