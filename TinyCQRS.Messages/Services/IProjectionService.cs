using System;
using TinyCQRS.Contracts.Commands;

namespace TinyCQRS.Contracts.Services
{
	public interface IProjectionService : IService
	{
		void RegenerateForSite(RegenerateForSite command);
	}
}