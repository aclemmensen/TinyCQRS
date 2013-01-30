using System;
using TinyCQRS.Messages.Shared;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.ReadModel.Model
{
	public class ResourceDto : IDto
	{
		public Guid Id { get; set; }

		public string Url { get; set; }
		public ResourceType Type { get; set; }

		public ResourceDto() { }

		public ResourceDto(Guid id, ResourceType type)
		{
			Id = id;
			Type = type;
		}
	}
}