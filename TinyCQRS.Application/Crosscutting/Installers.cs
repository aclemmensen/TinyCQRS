using System.Data.Entity;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using TinyCQRS.Application.Modules.Administration;
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

			container.Register(Classes.FromAssemblyContaining(typeof (IHandle<>)).BasedOn<IHandler>().WithServiceAllInterfaces()); //.Configure(x => x.LifeStyle.HybridPerWebRequestPerThread()));
			container.Register(Component.For<IMessageBus>().ImplementedBy<InMemoryMessageBus>()/*.LifeStyle.HybridPerWebRequestPerThread()*/);
			container.Register(Component.For(typeof(ICache<>)).ImplementedBy(typeof(MemoryCache<>))/*.LifeStyle.HybridPerWebRequestPerThread()*/);
			container.Register(Component.For(typeof(IRepository<>)).ImplementedBy(typeof(EventedRepository<>))/*.LifeStyle.HybridPerWebRequestPerThread()*/);
			container.Register(Component.For(typeof(ISagaRepository<>)).ImplementedBy(typeof(SagaRepository<>))/*.LifeStyle.HybridPerWebRequestPerThread()*/);
			container.Register(Component.For<ICommandDispatcher>().ImplementedBy<CommandDispatcher>()/*.LifeStyle.HybridPerWebRequestPerThread()*/);
			container.Register(Component.For<IBlobService>().ImplementedBy<BlobService>());
			container.Register(Component.For<ILogger>().ImplementedBy<TraceLogger>());
			container.Register(Component.For<IWindsorContainer>().Instance(container));
			container.Register(Component.For<IResolver>().ImplementedBy<WindsorResolverAdapter>());

			container.Register(Component.For(typeof(IEventStore)).ImplementedBy(typeof(CachingEventStore))/*.LifeStyle.HybridPerWebRequestPerThread()*/);

			container.Register(
				Classes.FromAssemblyContaining<SiteReadModelGenerator>().BasedOn<IConsume>().WithServiceAllInterfaces(),//.Configure(x => x /*.LifeStyle.HybridPerWebRequestPerThread()*/),
				Classes.FromAssemblyContaining<CrawlCoordinator>().BasedOn<IConsume>().WithServiceAllInterfaces(),//.Configure(x => x /*.LifeStyle.HybridPerWebRequestPerThread()*/),
				Classes.FromAssemblyContaining<Crawler>().BasedOn<IConsume>().WithServiceAllInterfaces());//.Configure(x => x/*.LifeStyle.HybridPerWebRequestPerThread()*/));

			container.Register(Classes
				.FromAssemblyContaining<SiteCrawlService>()
				.BasedOn<IService>()
				.WithServiceDefaultInterfaces());
				//.Configure(x => x.LifeStyle.HybridPerWebRequestPerThread()));
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

			container.Register(Component.For<IBlobStorage>().ImplementedBy<MongoBlobStorage>().DependsOn(new { connstr = _mongoConnstr }));

			//container.Register(Component.For<IEventStore>().ImplementedBy<SqlEventStore>().DependsOn(new { connstr = _eventConnstr })/*.LifeStyle.HybridPerWebRequestPerThread()*/);
			container.Register(Component.For<IEventStore>().ImplementedBy<OrmLiteEventStore>().DependsOn(new { connstr = _eventConnstr })/*.LifeStyle.HybridPerWebRequestPerThread()*/);
			//container.Register(Component.For<DbContext>().ImplementedBy<ReadModelContext>().LifeStyle.HybridPerWebRequestTransient());

			container.Register(
				//Component.For(typeof (IReadModelRepository<>)).ImplementedBy(typeof (CachingReadModelRepository<>)).LifeStyle.PerWebRequest,
				Component.For(typeof(IReadModelRepository<>)).ImplementedBy(typeof(MongoReadModelRepository<>)).DependsOn(new { connstr = _mongoConnstr })/*.LifeStyle.HybridPerWebRequestPerThread()*/);
				//Component.For(typeof (IReadModelRepository<>)).ImplementedBy(typeof (OrmLiteReadModelRepository<>)).DependsOn(new { connstr = _readConnstr }).LifeStyle.HybridPerWebRequestPerThread);
				//Component.For(typeof (IReadModelRepository<>)).ImplementedBy(typeof (EfReadModelRepository<>)).LifeStyle.PerWebRequest);
		}
	}

	public class MemoryServiceInstaller : ServiceInstaller
	{
		public override void Install(IWindsorContainer container, IConfigurationStore store)
		{
			base.Install(container, store);

			container.Register(Component.For(typeof(IEventStore)).ImplementedBy(typeof(InMemoryEventStore)));
			container.Register(Component.For(typeof(IReadModelRepository<>)).ImplementedBy(typeof(InMemoryReadModelRepository<>)));
		}
	}
}