using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Castle.Core.Internal;
using Castle.Core.Logging;
using Castle.Facilities.FactorySupport;
using Castle.Facilities.Logging;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using NHibernate;
using Routine.Core;
using Routine.Mvc;
using Routine.Soa;
using Routine.Test.Common;
using Routine.Test.Domain.NHibernate;
using Routine.Test.Domain.Windsor;

namespace Routine.Test.Domain.Configuration
{
	public static class Configurer
	{
		public static void ConfigureMvcApplication() { new Configuration().MvcApplication(); }
		public static void ConfigureSoaApplication() { new Configuration().SoaApplication(); }

		private class Configuration
		{
			private readonly IWindsorContainer container;

			public Configuration()
			{
				container = new WindsorContainer();
				container
					.AddFacility<FactorySupportFacility>()
					.AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.Trace).WithLevel(LoggerLevel.Debug));

				#if DEBUG
				container.Kernel.ComponentRegistered += (k, h) => h.ComponentModel.Services.ForEach(t => Console.WriteLine(t.FullName + ", " + h.ComponentModel.Implementation.FullName));
				#endif

				ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container.Kernel));
			}

			public void MvcApplication()
			{
				container.Register(
					Component.For<IMvcContext>()
						.Instance(BuildRoutine.Context()
							.AsMvcApplication(MvcConfiguration(), CodingStyle()))
						.LifestyleSingleton(),

					Component.For<MvcController>().ImplementedBy<MvcController>().LifestylePerWebRequest()
				);

				Logging();
				Modules();
				DataAccess();
			}

			public void SoaApplication()
			{
				container.Register(
					Component.For<ISoaContext>()
						.Instance(BuildRoutine.Context()
							.UsingInterception(ServerInterceptionConfiguration())
							.AsSoaApplication(SoaConfiguration(), CodingStyle()))
						.LifestyleSingleton(),

					Component.For<SoaController>().ImplementedBy<SoaController>().LifestylePerWebRequest()
				);

				Logging();
				Modules();
				DataAccess();
			}

			#region log4net
			private void Logging()
			{
				var root = ((Hierarchy)LogManager.GetRepository()).Root;

				root.AddAppender(DebugAppender());

				SetLevel("NHibernate", Level.Warn);
				SetLevel("NHibernate.SQL", Level.Debug);
				SetLevel("NHibernate.Caches", Level.Debug);

				root.Repository.Configured = true;
			}

			private void SetLevel(string logger, Level level)
			{
				((Logger)LogManager.GetLogger(logger).Logger).Level = level;
			}

			private FileAppender FileAppender(string fileName, Level threshold)
			{
				var appender = new RollingFileAppender
				{
					Name = "RollingFile",
					AppendToFile = true,
					File = @"logs\" + fileName,
					Layout = new PatternLayout("%date %-5level %logger - %message%newline"),
					Threshold = threshold,
					RollingStyle = RollingFileAppender.RollingMode.Date,
					DatePattern = ".yyyyMMdd",
					StaticLogFileName = true,
					MaximumFileSize = "2000KB"
				};

				appender.ActivateOptions();
				return appender;
			}

			private DebugAppender DebugAppender()
			{
				var appender = new DebugAppender
				{
					Name = "Debug",
					Layout = new PatternLayout("%-5level - %message%newline"),
					Threshold = Level.Debug
				};

				appender.ActivateOptions();
				return appender;
			}
			#endregion

			private ICodingStyle CodingStyle()
			{
				return BuildRoutine.CodingStyle()
						.FromBasic()
						.DomainTypeRootNamespacesAre("Routine.Test.Module")
						.Use(p => p.NullPattern("_null"))
						.Use(p => p.ShortModelIdPattern("System", "s"))
						.Use(p => p.ShortModelIdPattern("Routine.Test.Common", "c"))
						.Use(p => p.ShortModelIdPattern("Routine.Test.Module", "m"))
						.Use(p => p.ParseableValueTypePattern())
						.Use(p => p.EnumPattern())
						.Use(p => p.SingletonPattern(container, "Instance"))
						.Use(p => p.AutoMarkWithAttributesPattern())

						.SelectModelMarks.Done(s => s.Always("Module").When(t => container.TypeIsSingleton(t) && t.FullName.EndsWith("Module")))
						.SelectModelMarks.Done(s => s.Always("Search").When(t => t.CanBe<IQuery>()))
						.SelectOperationMarks.Done(s => s.Always("ParamOptions").When(o => o.HasNoParameters() && o.ReturnsCollection() && o.Name.Matches("GetAvailable.*sFor.*")))

						.ExtractModelModule.Done(e => e.ByConverting(t => t.Namespace.After("Module.").BeforeLast("."))
												  .When(t => t.IsDomainType))

						.SelectInitializers.Done(i => i.ByPublicConstructors().When(t => t.IsValueType && t.IsDomainType))
						.SelectMembers.Done(s => s.ByPublicProperties(p => p.IsWithinRootNamespace(true) && p.IsPubliclyReadable && !p.IsIndexer && !p.Returns<Guid>() && !p.Returns<TypedGuid>() && !p.ReturnsCollection())
												  .When(t => t.IsDomainType))

						.SelectOperations.Done(s => s.ByPublicMethods(m => m.IsWithinRootNamespace(true) && m.GetParameters().All(p => p.ParameterType != type.of<Guid>() && p.ParameterType != type.of<TypedGuid>()))
													 .When(t => t.IsDomainType))
						.SelectOperations.Done(s => s.ByPublicProperties(p => p.ReturnsCollection())
													 .When(t => t.IsDomainType))

						.ExtractValue.Add(e => e.ByPublicProperty(p => p.Returns<string>("Title")))
									 .Add(e => e.ByPublicProperty(p => p.Returns<string>("Name")))
									 .Add(e => e.ByConverting(o => o.GetType().Name.BeforeLast("Module").SplitCamelCase().ToUpperInitial())
											    .WhenType(t => container.TypeIsSingleton(t)))
									 .Done(e => e.ByConverting(o => string.Format("{0}", o)))

						.Use(p => p.FromEmpty()
							.ExtractId.Done(e => e.ByProperty(pr => Orm.IsId(pr))
												  .WhenType(t => Orm.ShouldMap(t))
												  .ReturnAsString())
							.Locate.Done(l => l.By((t, id) => container.Resolve<ISession>().Get(t.GetActualType(), Guid.Parse(id)))
											   .AcceptNullResult(false)
											   .WhenType(t => Orm.ShouldMap(t))))
						;
			}

			private ISoaConfiguration SoaConfiguration()
			{
				return BuildRoutine.SoaConfig()
						.FromBasic()
						.DefaultParametersAre("language_code")
						.MaxResultLengthIs(100000000)
						.Use(p => p.ExceptionsWrappedAsUnhandledPattern())
						;
			}

			private IInterceptionConfiguration ServerInterceptionConfiguration()
			{
				return BuildRoutine.InterceptionConfig()
						.FromBasic()
						.Use(p => p.CommonInterceptorPattern(i => i.Before(() => Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(HttpContext.Current.Request["language_code"] ?? "en-US"))))
						.Use(p => p.CommonInterceptorPattern(i => i.ByDecorating(() => container.BeginScope()).After(s => s.Dispose())))

						.InterceptPerformOperation
							.Add(i => i.Do()
									.Before(ctx => Debug.WriteLine(string.Format("performing -> {0}", ctx.OperationModelId)))
									.Success(ctx => Debug.WriteLine(string.Format("\treturns -> {0}", ctx.Result)))
									.Fail(ctx => Debug.WriteLine(string.Format("\tthrows -> {0}", ctx.Exception)))
									.After(ctx => Debug.WriteLine(string.Format("end of {0}", ctx.OperationModelId))))
							.Done(i => i.Before(() => { throw new Exception("Cannot call hidden service"); })
									   .When(ctx => ctx.GetOperationModel().Marks.Contains("Hidden")))

						.Use(p => p.CommonInterceptorPattern(i => i.ByDecorating(ctx => container.Resolve<ISession>().BeginTransaction())
																   .Success(t => t.Commit())
																   .Fail(t => t.Rollback())))
						;
			}

			private IMvcConfiguration MvcConfiguration()
			{
				return BuildRoutine.MvcConfig()
						.FromBasic("Instance")
						.Use(p => p.CommonInterceptorPattern(i => i.ByDecorating(() => container.BeginScope()).After(s => s.Dispose())))
						.InterceptPerform
							.Done(i => i.Do()
											.Before(ctx => Debug.WriteLine(string.Format("performing -> {0}", ctx.OperationModelId)))
											.Success(ctx => Debug.WriteLine(string.Format("\treturns -> {0}", ctx.Result)))
											.Fail(ctx => Debug.WriteLine(string.Format("\tthrows -> {0}", ctx.Exception)))
											.After(ctx => Debug.WriteLine(string.Format("end of {0}", ctx.OperationModelId))))
						.Use(p => p.CommonInterceptorPattern(i => i.ByDecorating(() => container.Resolve<ISession>().BeginTransaction())
																   .Success(t => t.Commit())
																   .Fail(t => t.Rollback())))

						.ExtractIndexId.Done(e => e.Always("Instance").When(om => !om.IsViewModel && om.Marks.Contains("Module")))
						.ExtractMenuIds
							.Add(e => e.Always("Instance").When(om => !om.IsViewModel && om.Marks.Contains("Search")))
							.Done(e => e.Always("Instance").When(om => !om.IsViewModel && om.Marks.Contains("Module")))

						.ExtractViewName
							.Add(e => e.Always("Search").When(vmb => vmb is ObjectViewModel && ((ObjectViewModel)vmb).MarkedAs("Search")))
							.Done(e => e.ByConverting(vmb => vmb.GetType().Name.Before("ViewModel")))

						.ExtractParameterDefault.Done(e => e.ByConverting(p => p.Operation.Object[p.Id.ToUpperInitial()].GetValue())
													 .When(p => p.Operation.Id.Matches("Update.*") && p.Operation.Object.Members.Any(m => m.Id == p.Id.ToUpperInitial())))

						.ExtractParameterOptions
							.Add(e => e.ByConverting(p => p.Operation.Object.Perform("GetAvailable" + p.Id.ToUpperInitial() + "sFor" + p.Operation.Id).List)
									   .When(p => p.Operation.Object.Operations.Any(o => o.Id == "GetAvailable" + p.Id.ToUpperInitial() + "sFor" + p.Operation.Id && 
																						 o.ResultIsList && !o.Parameters.Any())))
							.Add(e => e.ByConverting(p => p.Operation.Object.Application.Get("Instance", p.ViewModelId + "s").Perform("All").List)
									   .When(p => p.Operation.Object.Application.ObjectModels.Any(m => m.Id == p.ViewModelId + "s" && m.Operations.Any(o => o.Id == "All"))))
							.Done(e => e.ByConverting(p => p.Operation.Object.Application.GetAvailableObjects(p.ViewModelId)))

						.ExtractDisplayName.Done(e => e.ByConverting(s => s.SplitCamelCase().ToUpperInitial()))
						.ExtractOperationOrder.Done(e => e.Always(ovm => ovm.HasParameter ?0:1))
						;
			}

			private void Modules()
			{
				container.Register(
					Component.For<IDomainContext>().ImplementedBy<WindsorDomainContext>().LifestyleSingleton(),
					Classes
						.FromAssemblyInDirectory(ModuleDirectory)
						.AllowMultipleMatches()
						.Where(TypeShouldBeSingleton)
						.WithServiceSelf()
						.WithServiceAllInterfaces()
						.LifestyleSingleton(),
					Classes
						.FromAssemblyInDirectory(ModuleDirectory)
						.AllowMultipleMatches()
						.Where(TypeShouldBeTransient)
						.WithServiceSelf()
						.WithServiceAllInterfaces()
						.LifestyleTransient());
			}

			private AssemblyFilter BinDirectory { get { return new AssemblyFilter("bin"); } }
			private AssemblyFilter ModuleDirectory { get {return BinDirectory.FilterByName(n => n.Name.StartsWith("Routine.Test.Module"));} }

			private bool TypeShouldBeSingleton(Type type)
			{
				return (typeof(IQuery).IsAssignableFrom(type) || type.Name.EndsWith("Module")) && 
					type.IsClass && !type.IsAbstract;
			}

			private bool TypeShouldBeTransient(Type type) { return !TypeShouldBeSingleton(type); }

			private void DataAccess()
			{
				container.Register(
					Component.For<ISessionFactory>()		.Instance(BuildSessionFactory())				.LifestyleSingleton(),
					Component.For<ISession>()				.UsingFactoryMethod(OpenSession)				.LifestyleScoped(),
					Component.For(typeof(IRepository<>))	.ImplementedBy(typeof(NHibernateRepository<>))	.LifestyleScoped(),
					Component.For(typeof(ILookup<>))		.ImplementedBy(typeof(NHibernateLookup<>))		.LifestyleScoped());
			}
			
			private OrmConfiguration Orm { get { return OrmConfiguration.Instance; } }

			private ISessionFactory BuildSessionFactory()
			{
				return Orm.BuildSessionFactory(container.Resolve<IDomainContext>(), ModuleAssemblies());
			}

			private IEnumerable<System.Reflection.Assembly> ModuleAssemblies()
			{
				var result = ReflectionUtil.GetAssemblies(ModuleDirectory);

				#if DEBUG
				Debug.WriteLine("assembly count: " + result.Count());
				foreach(var item in result)
				{
					Debug.WriteLine("module: " + item.FullName);
				}
				#endif

				return result;
			}

			private ISession OpenSession(IKernel kernel)
			{
				return kernel.Resolve<ISessionFactory>().OpenSession();
			}
		}
	}
}

