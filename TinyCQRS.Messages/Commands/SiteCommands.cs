using System;

namespace TinyCQRS.Messages.Commands
{
	public class CreateNewSite : Command
	{
		public string Name { get; private set; }
		public string Root { get; private set; }

		public CreateNewSite(Guid siteId, string name, string root) : base(siteId)
		{
			Name = name;
			Root = root;
		}
	}

	public class AddPageToSite : Command
	{
		public Guid PageId { get; private set; }

		public AddPageToSite(Guid siteId, Guid pageId) : base(siteId)
		{
			PageId = pageId;
		}
	}
}