using System;

namespace TinyCQRS.Contracts.Commands
{
	public class RegenerateForSite : Command
	{
		public RegenerateForSite(Guid siteId) : base(siteId)
		{
		}
	}
}