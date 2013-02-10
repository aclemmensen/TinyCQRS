using System.Data.Entity;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using TinyCQRS.Application.Modules.Crawler;
using TinyCQRS.Contracts;
using TinyCQRS.Contracts.Services;
using TinyCQRS.Domain.Interfaces;
using TinyCQRS.Domain.Models.QualityAssurance;
using TinyCQRS.Infrastructure;
using TinyCQRS.Infrastructure.Caching;
using TinyCQRS.Infrastructure.Persistence;
using TinyCQRS.ReadModel.Generators;
using TinyCQRS.ReadModel.Infrastructure;
using TinyCQRS.ReadModel.Interfaces;

namespace TinyCQRS.Application.Crosscutting
{
	public abstract class ServiceInstaller : IWindsorInstaller
	{
		protected IWindsorContainer _container;

		public virtual void Install(IWindsorContainer container, IConfigurationStore store)
		{
			_container = container;
			
			container.Register(Classes.FromAssemblyContaining(typeof (IHandle<>)).BasedOn<IHandler>().WithServiceAllInterfaces().LifestylePerWebRequest());
			container.Register(Component.For<IMessageBus>().ImplementedBy<InMemoryMessageBus>().LifeStyle.PerWebRequest);
			container.Register(Component.For(typeof (ICache<>)).ImplementedBy(typeof (MemoryCache<>)).LifeStyle.PerWebRequest);
			container.Register(Component.For(typeof (IRepository<>)).ImplementedBy(typeof (EventedAggregateRepository<>)).LifeStyle.PerWebRequest);
			container.Register(Component.For(typeof (ISagaRepository<>)).ImplementedBy(typeof (EventedSagaRepository<>)).LifeStyle.PerWebRequest);
			container.Register(Component.For<ICommandDispatcher>().ImplementedBy<CommandDispatcher>().LifeStyle.PerWebRequest);
			container.Register(Component.For<ILogger>().ImplementedBy<TraceLogger>());
			container.Register(Component.For<IWindsorContainer>().Instance(container));
			container.Register(Component.For<IResolver>().ImplementedBy<WindsorResolverAdapter>());

			container.Register(Component.For(typeof(IEventStore<>)).ImplementedBy(typeof(CachingEventStore<>)).LifeStyle.PerWebRequest);

			container.Register(
				Classes.FromAssemblyContaining<SiteReadModelGenerator>().BasedOn<IConsume>().WithServiceAllInterfaces().LifestylePerWebRequest(),
				Classes.FromAssemblyContaining<CrawlJobCommandHandler>().BasedOn<IConsume>().WithServiceAllInterfaces().LifestylePerWebRequest(),
				Classes.FromAssemblyContaining<Crawler>().BasedOn<IConsume>().WithServiceAllInterfaces().LifestylePerWebRequest());

			container.Register(Classes
				.FromAssemblyContaining<SiteCrawlService>()
				.BasedOn<IService>()
				.WithServiceDefaultInterfaces()
				.LifestylePerWebRequest());
		}
	}

	public class DatabaseServiceInstaller : ServiceInstaller
	{
		private readonly string _eventConnstr;
		private readonly string _readConnstr;
		private readonly string _mongoConnstr;

		public DatabaseServiceInstaller(string eventConnstr, string readConnstr, string mongoConnstr)
		{
			_eventConnstr = eventConnstr;
			_readConnstr = readConnstr;
			_mongoConnstr = mongoConnstr;
		}

		public override void Install(IWindsorContainer container, IConfigurationStore store)
		{
			base.Install(container, store);

			container.Register(Component.For(typeof(IEventStore<>)).ImplementedBy(typeof(SqlEventStore<>)).DependsOn(new { connstr = _eventConnstr }).LifeStyle.PerWebRequest);
			container.Register(Component.For<DbContext>().ImplementedBy<ReadModelContext>().LifeStyle.PerWebRequest);

			container.Register(
				//Component.For(typeof (IReadModelRepository<>)).ImplementedBy(typeof (CachingReadModelRepository<>)).LifeStyle.PerWebRequest,
				Component.For(typeof(IReadModelRepository<>)).ImplementedBy(typeof(MongoReadModelRepository<>)).DependsOn(new { connstr = _mongoConnstr }).LifeStyle.PerWebRequest);
				//Component.For(typeof (IReadModelRepository<>)).ImplementedBy(typeof (OrmLiteReadModelRepository<>)).DependsOn(new { connstr = _readConnstr }).LifeStyle.PerWebRequest);
				//Component.For(typeof (IReadModelRepository<>)).ImplementedBy(typeof (EfReadModelRepository<>)).LifeStyle.PerWebRequest);
		}
	}

	public class MemoryServiceInstaller : ServiceInstaller
	{
		public override void Install(IWindsorContainer container, IConfigurationStore store)
		{
			base.Install(container, store);

			container.Register(Component.For(typeof(IEventStore<>)).ImplementedBy(typeof(InMemoryEventStore<>)));
			container.Register(Component.For(typeof(IReadModelRepository<>)).ImplementedBy(typeof(InMemoryReadModelRepository<>)));
		}
	}
}