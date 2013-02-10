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
			var events = _eventStore.GetEventsFor(command.AggregateId);

			// But crawls aren't attached to the site aggregate, so this won't even work.
			// Also, the RMGs are recreating DTOs that already exist, causing duplicate ID
			// errors.

			foreach (var @event in events)
			{
				_bus.Notify(@event);
			}
		}
	}
}