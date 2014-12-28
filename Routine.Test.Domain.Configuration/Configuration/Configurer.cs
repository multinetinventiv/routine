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
using Routine.Engine;
using Routine.Engine.Virtual;
using Routine.Interception;
using Routine.Soa;
using Routine.Test.Common;
using Routine.Test.Domain.NHibernate;
using Routine.Test.Domain.Windsor;
using Routine.Ui;
using InterceptionTarget = Routine.Ui.InterceptionTarget;

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

			#region file appender sample
			//private FileAppender FileAppender(string fileName, Level threshold)
			//{
			//	var appender = new RollingFileAppender
			//	{
			//		Name = "RollingFile",
			//		AppendToFile = true,
			//		File = @"logs\" + fileName,
			//		Layout = new PatternLayout("%date %-5level %logger - %message%newline"),
			//		Threshold = threshold,
			//		RollingStyle = RollingFileAppender.RollingMode.Date,
			//		DatePattern = ".yyyyMMdd",
			//		StaticLogFileName = true,
			//		MaximumFileSize = "2000KB"
			//	};

			//	appender.ActivateOptions();
			//	return appender;
			//} 
			#endregion

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
						.AddTypes(ModuleAssemblies(), t => t.Namespace.StartsWith("Routine.Test.Module") && t.IsPublic)
						.Use(p => p.VirtualTypePattern())
						.AddTypes(v => v.FromBasic()
							.Namespace.Set("Routine.Test.Module.Virtual")
							.Name.Set("VirtualType")
							.Operations.Add(o => o.Proxy<int>().TargetByParameter("i"))
							.Operations.Add(o => o.Virtual("VirtualService", (string str, int i) => string.Join(",", Enumerable.Range(0, i).Select(ix => str))))
						)
						.StaticInstances.Set(c => c.By(t => new VirtualObject("Instance", t as VirtualType)).When(t => t is VirtualType))
						.Use(p => p.ShortModelIdPattern("System", "s"))
						.Use(p => p.ShortModelIdPattern("Routine.Test.Common", "c"))
						.Use(p => p.ShortModelIdPattern("Routine.Test.Module", "m"))
						.Use(p => p.ParseableValueTypePattern())
						.Use(p => p.EnumPattern(false))
						.Use(p => p.SingletonPattern(container, "Instance"))
						.Use(p => p.AutoMarkWithAttributesPattern())

						.TypeMarks.Add("Module", t => t is TypeInfo && container.TypeIsSingleton(t) && t.FullName.EndsWith("Module"))
						.TypeMarks.Add("Search", t => t.CanBe<IQuery>())
						.OperationMarks.Add("ParamOptions", o => o.HasNoParameters() && o.ReturnsCollection() && o.Name.Matches("GetAvailable.*sFor.*"))
						.OperationMarks.Add("Virtual", o => o is VirtualOperation || o is ProxyOperation)

						.Module.Set(c => c.By(t => t.Namespace.After("Module.").BeforeLast(".")).When(t => t.IsDomainType))

						.Initializers.Add(c => c.PublicInitializers().When(t => t.IsValueType && t.IsDomainType))
						.Members.Add(c => c.PublicMembers(m => !m.IsInherited(true, true) && m.IsPublic && !m.Returns<Guid>() && !m.Returns<TypedGuid>())
										   .When(t => t.IsDomainType))

						.Operations.Add(c => c.PublicOperations(o => !o.IsInherited(true, true) && o.Parameters.All(p => !p.ParameterType.Equals(type.of<Guid>()) && !p.ParameterType.Equals(type.of<TypedGuid>())))
											  .When(t => t.IsDomainType))
						.Operations.Add(c => c.Build(o => o.Proxy<string>("Replace").TargetByParameter("str")).When(t => t.IsDomainType && !t.IsValueType && !t.IsInterface))
						.Operations.Add(c => c.Build(o => o.Virtual("Concat", (string str1, string str2) => str1 + str2)).When(t => t.IsDomainType && !t.IsValueType && !t.IsInterface))

						.ValueExtractor.Set(c => c.ValueByMember(m => m.Returns<string>("Title")))
						.ValueExtractor.Set(c => c.ValueByMember(m => m.Returns<string>("Name")))
						.ValueExtractor.Set(c => c.Value(e => e.By(o => o.GetType().Name.BeforeLast("Module").SplitCamelCase().ToUpperInitial()))
												  .When(t => t is TypeInfo && container.TypeIsSingleton(t)))
						.ValueExtractor.Set(c => c.Value(e => e.By(o => string.Format("{0}", o))))

						.Use(p => p.FromEmpty()
							.IdExtractor.Set(c => c.Id(e => e.By(o => container.Resolve<ISession>().GetIdentifier(o).ToString()))
												   .When(t => t is TypeInfo && Orm.IsPersistent((TypeInfo)t)))
							.ObjectLocator.Set(c => c.Locator(l => l.By((t, id) => container.Resolve<ISession>().Get(((TypeInfo)t).GetActualType(), Guid.Parse(id))).AcceptNullResult(false))
													 .When(t => t is TypeInfo && Orm.IsPersistent((TypeInfo)t)))
							)
						;
			}

			private ISoaConfiguration SoaConfiguration()
			{
				return BuildRoutine.SoaConfig()
						.FromBasic()
						.Headers.Add("language_code")
						.MaxResultLength.Set(100000000)
						.Use(p => p.ExceptionsWrappedAsUnhandledPattern())
						;
			}

			private IInterceptionConfiguration ServerInterceptionConfiguration()
			{
				return BuildRoutine.InterceptionConfig()
						.FromBasic()
						.Interceptors.Add(c => c.Interceptor(i => i.Before(() => Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(HttpContext.Current.Request["language_code"] ?? "en-US"))))
						.Interceptors.Add(c => c.Interceptor(i => i.ByDecorating(() => container.BeginScope()).After(s => s.Dispose())))
						.Interceptors.Add(p => p.Interceptor(i => i.ByDecorating(ctx => container.Resolve<ISession>().BeginTransaction())
																   .Success(t => t.Commit())
																   .Fail(t => t.Rollback())))

						.ServiceInterceptors.Add(c => c.Interceptor(i => i.Before(ctx => Debug.WriteLine(string.Format("performing -> {0}", ctx.OperationModelId)))
																		  .Success(ctx => Debug.WriteLine(string.Format("\treturns -> {0}", ctx.Result)))
																		  .Fail(ctx => Debug.WriteLine(string.Format("\tthrows -> {0}", ctx.Exception)))
																		  .After(ctx => Debug.WriteLine(string.Format("end of {0}", ctx.OperationModelId)))))
						.ServiceInterceptors.Add(c => c.Interceptor(i => i.Before(() => { throw new Exception("Cannot call hidden service"); }))
													   .When(iom => iom.OperationModel.Marks.Contains("Hidden")))
						;
			}

			private IMvcConfiguration MvcConfiguration()
			{
				return BuildRoutine.MvcConfig()
						.FromBasic("Instance")
						.Interceptors.Add(c => c.Interceptor(i => i.ByDecorating(() => container.BeginScope()).After(s => s.Dispose())))
						.Interceptors.Add(c => c.Interceptor(i => i.Do()
													.Before(ctx => Debug.WriteLine(string.Format("performing -> {0}", ctx["OperationModelId"])))
													.Success(ctx => Debug.WriteLine(string.Format("\treturns -> {0}", ctx.Result)))
													.Fail(ctx => Debug.WriteLine(string.Format("\tthrows -> {0}", ctx.Exception)))
													.After(ctx => Debug.WriteLine(string.Format("end of {0}", ctx["OperationModelId"]))))
												.When(s => s == InterceptionTarget.Perform))
						.Interceptors.Add(c => c.Interceptor(i => i.ByDecorating(() => container.Resolve<ISession>().BeginTransaction())
																   .Success(t => t.Commit())
																   .Fail(t => t.Rollback())))

						.IndexId.Set("Instance", om => !om.IsViewType && om.MarkedAs("Module"))
						.MenuIds.Add("Instance", om => !om.IsViewType && om.MarkedAs("Search"))
						.MenuIds.Add("Instance", om => !om.IsViewType && om.MarkedAs("Module"))

						.ViewName.Set("Search", vm => vm is ObjectViewModel && ((ObjectViewModel)vm).MarkedAs("Search"))
						.ViewName.Set(c => c.By(vm => vm.GetType().Name.Before("ViewModel")))

						.ParameterDefault.Set(c => c.By(p => p.Target[p.Id.ToUpperInitial()].Get())
													.When(p => p.Parameter.Owner.IsOperation() && p.Parameter.Owner.Operation.Id.Matches("Update.*") && p.Parameter.Type.Members.Any(m => m.Id == p.Id.ToUpperInitial())))

						.ParameterOptions.Set(c => c.By(p => p.Target.Perform("GetAvailable" + p.Id.ToUpperInitial() + "sFor" + p.Parameter.Owner.Operation.Id).List)
													.When(p => p.Parameter.Owner.IsOperation() && 
															   p.Parameter.Type.Operations.Any(o => o.Id == "GetAvailable" + p.Id.ToUpperInitial() + "sFor" + p.Parameter.Owner.Operation.Id &&
																									o.ResultIsList && !o.Parameters.Any())))
						.ParameterOptions.Set(c => c.By(p => p.Parameter.Application.Get("Instance", p.Parameter.ParameterType.Id + "s").Perform("All").List)
													.When(p => p.Parameter.Application.Types.Any(t => t.Id == p.Parameter.ParameterType.Id + "s" && t.Operations.Any(o => o.Id == "All"))))

						.ParameterOptions.Set(c => c.By(p => p.Parameter.ParameterType.StaticInstances))

						.ParameterSearcher.Set(c => c.By(p =>
						{
							var som = p.Application.Types.SingleOrDefault(t => t.Module == p.ParameterType.Module && t.Name == p.ParameterType.Name + "s");

							if (som == null)
							{
								return null;
							}

							return p.Application.Get("Instance", som.Id);
						}))

						.DisplayName.Set(c => c.By(s => s.SplitCamelCase().ToUpperInitial()))
						.OperationOrder.Set(c => c.By(ovm => ovm.ViewModel.HasParameter ? 0 : 1))

						.OperationIsRendered.Set(false, o => o.MarkedAs("ParamOptions"))
						.OperationIsRendered.Set(false, o => o.MarkedAs("Virtual"))
						.OperationTypes.Set(OperationTypes.Search, o => o.ReturnsList)
						.OperationTypes.Set(OperationTypes.Page | OperationTypes.Table)
						.ConfirmationRequired.Set(false, o => o.Id.StartsWith("Test"))
						.ConfirmationRequired.Set(false, o => o.Id.StartsWith("Mark"))

						.MemberIsRendered.Set(false, m => m.MarkedAs("Virtual"))
						.MemberTypes.Set(MemberTypes.PageTable, m => m.IsList)
						.MemberTypes.Set(MemberTypes.PageNameValue | MemberTypes.TableColumn, m => !m.IsList)
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
			private AssemblyFilter ModuleDirectory { get { return BinDirectory.FilterByName(n => n.Name.StartsWith("Routine.Test.Module")); } }

			private bool TypeShouldBeSingleton(Type type)
			{
				return (typeof(IQuery).IsAssignableFrom(type) || type.Name.EndsWith("Module")) &&
					type.IsClass && !type.IsAbstract;
			}

			private bool TypeShouldBeTransient(Type type) { return !TypeShouldBeSingleton(type); }

			private void DataAccess()
			{
				container.Register(
					Component.For<ISessionFactory>().Instance(BuildSessionFactory()).LifestyleSingleton(),
					Component.For<ISession>().UsingFactoryMethod(OpenSession).LifestyleScoped(),
					Component.For(typeof(IRepository<>)).ImplementedBy(typeof(NHibernateRepository<>)).LifestyleScoped(),
					Component.For(typeof(ILookup<>)).ImplementedBy(typeof(NHibernateLookup<>)).LifestyleScoped());
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
				foreach (var item in result)
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

