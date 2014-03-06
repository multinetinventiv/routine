using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Routine.Api;
using Routine.Core;
using Routine.Core.Cache;
using Routine.Core.CoreContext;
using Routine.Core.Rest;
using Routine.Core.Service;
using Routine.Core.Service.Impl;
using Routine.Mvc;
using Routine.Soa;

namespace Routine.Windsor
{
	public static class WindsorInstallation
	{
		public static IWindsorContainer InstallMvcClient(this IWindsorContainer container, IMvcConfiguration mvcConfiguration, ISoaClientConfiguration soaClientConfiguration)
		{
			container.InstallBaseCommon();
			container.InstallSoaClientCommon();
			container.InstallMvcCommon();

			container.Register(
				Component.For<IMvcConfiguration>()		.Instance(mvcConfiguration)					.LifestyleSingleton(),
				Component.For<ISoaClientConfiguration>().Instance(soaClientConfiguration)			.LifestyleSingleton(),
				Component.For<IObjectService>()			.ImplementedBy<ObjectServiceRestClient>()	.LifestyleSingleton()
			);

			return container;
		}

		public static IWindsorContainer InstallSoaClient(this IWindsorContainer container, ISoaClientConfiguration soaClientConfiguration)
		{
			container.InstallBaseCommon();
			container.InstallSoaClientCommon();

			container.Register(
				Component.For<ISoaClientConfiguration>().Instance(soaClientConfiguration)			.LifestyleSingleton(),
				Component.For<IObjectService>()			.ImplementedBy<ObjectServiceRestClient>()	.LifestyleSingleton()
			);

			return container;
		}

		public static IWindsorContainer InstallSoaApplication(this IWindsorContainer container, ISoaConfiguration soaConfiguration, ICodingStyle codingStyle)
		{
			container.InstallBaseCommon();
			container.InstallSoaCommon();

			container.Register(
				Component.For<ISoaConfiguration>()		.Instance(soaConfiguration)				.LifestyleSingleton(),
				Component.For<ICodingStyle>()			.Instance(codingStyle)					.LifestyleSingleton(),
				Component.For<IObjectService>()			.ImplementedBy<ObjectService>()			.LifestyleSingleton(),
				Component.For<SoaController>()			.ImplementedBy<SoaController>()			.LifestyleTransient()
			);

			return container;
		}

		public static IWindsorContainer InstallMvcApplication(this IWindsorContainer container, IMvcConfiguration mvcConfiguration, ICodingStyle codingStyle)
		{
			container.InstallBaseCommon();
			container.InstallSoaCommon();
			container.InstallSoaClientCommon();
			container.InstallMvcCommon();

			container.Register(
				Component.For<IMvcConfiguration>()		.Instance(mvcConfiguration)				.LifestyleSingleton(),
				Component.For<ICodingStyle>()			.Instance(codingStyle)					.LifestyleSingleton(),
				Component.For<IObjectService>()			.ImplementedBy<ObjectService>()			.LifestyleSingleton()
			);

			return container;
		}

		private static IWindsorContainer InstallBaseCommon(this IWindsorContainer container)
		{
			container.Register(
				Component.For<IFactory>()				.ImplementedBy<WindsorFactory>()			.LifestyleSingleton(),
				Component.For<IRestClient>()			.ImplementedBy<WebRequestRestClient>()	.LifestyleSingleton()
			);

			return container;
		}

		private static IWindsorContainer InstallSoaCommon(this IWindsorContainer container)
		{
			container.Register(
				Component.For<ICoreContext>()			.ImplementedBy<CachedFactoryCoreContext>()	.LifestyleSingleton(),
				Component.For<ICache>()					.ImplementedBy<WebCache>()					.LifestyleSingleton()
			);

			container.Register(
				Classes.FromAssemblyContaining<DomainType>()
				.Where(t => t.Namespace.EndsWith("Core") && t.Name.StartsWith("Domain"))
				.WithServiceSelf()
				.LifestyleTransient());


			return container;
		}

		private static IWindsorContainer InstallSoaClientCommon(this IWindsorContainer container)
		{
			container.Register(
				Component.For<Rapplication>()			.ImplementedBy<Rapplication>()			.LifestyleSingleton()
			);

			container.Register(
				Classes.FromAssemblyContaining<Rapplication>()
				.Where(t => t.Namespace.EndsWith("Api") && t.Name.StartsWith("R") && t.Name != "Rapplication")
				.WithServiceSelf()
				.LifestyleTransient());

			return container;
		}

		private static IWindsorContainer InstallMvcCommon(this IWindsorContainer container)
		{
			container.Register(
				Component.For<ApplicationViewModel>()	.ImplementedBy<ApplicationViewModel>()	.LifestyleSingleton(),
				Component.For<MvcController>()			.ImplementedBy<MvcController>()			.LifestylePerWebRequest()
			);

			container.Register(
				Classes.FromAssemblyContaining<ApplicationViewModel>()
				.Where(t => t.Namespace.EndsWith("Mvc") && t.Name.EndsWith("ViewModel") && t.Name != "ApplicationViewModel")
				.WithServiceSelf()
				.LifestyleTransient());

			return container;
		}
	}
}

