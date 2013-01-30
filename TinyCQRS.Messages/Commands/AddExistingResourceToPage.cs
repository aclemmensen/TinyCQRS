using System;
using TinyCQRS.Messages.Shared;

namespace TinyCQRS.Messages.Commands
{
	public class AddExistingResourceToPage : Command
	{
		public Guid ResourceId { get; private set; }

		public AddExistingResourceToPage(Guid pageId, Guid resourceId) : base(pageId)
		{
			ResourceId = resourceId;
		}
	}

	public class AddNewResourceToPage : Command
	{
		public ResourceType Type { get; set; }
		public string Url { get; set; }

		public AddNewResourceToPage(Guid pageId, ResourceType type, string url) : base(pageId)
		{
			Type = type;
			Url = url;
		}
	}
}