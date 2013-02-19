using System;
using TinyCQRS.Contracts.Commands;
using TinyCQRS.Contracts.Services;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Domain.Models.QualityAssurance;
using TinyCQRS.Infrastructure;

namespace TinyCQRS.Application.Modules.Administration
{
	public class ProjectionService : IProjectionService
	{
		private readonly IEventStore _eventStore;
		private readonly IMessageBus _bus;

		public ProjectionService(IEventStore eventStore, IMessageBus bus)
		{
			_eventStore = eventStore;
			_bus = bus;
		}

		public void RegenerateForSite(RegenerateForSite command)
		{
			
		}
	}
}