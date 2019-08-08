using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
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
using Routine.Core.Rest;
using Routine.Engine;
using Routine.Engine.Virtual;
using Routine.Interception;
using Routine.Service;
using Routine.Test.Common;
using Routine.Test.Domain.NHibernate;
using Routine.Test.Domain.Windsor;

namespace Routine.Test.Domain.Configuration
{
	public static class Configurer
	{
		public static void ConfigureServiceApplication() { new Configuration().ServiceApplication(); }
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
			}

			public void ServiceApplication()
			{
				container.Register(
					Component.For<IServiceContext>()
						.Instance(BuildRoutine.Context()
							.UsingInterception(ServerInterceptionConfiguration())
							.UsingJavaScriptSerializer(maxJsonLength: int.MaxValue)
							.AsServiceApplication(ServiceConfiguration(), CodingStyle()))
						.LifestyleSingleton()
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
						.AddCommonSystemTypes()
						.AddTypes(typeof(Text).Assembly, t => t.IsPublic)
						.AddTypes(ModuleAssemblies(), t => t.Namespace != null && t.Namespace.StartsWith("Routine.Test.Module") && t.IsPublic)
						.Use(p => p.VirtualTypePattern())
						.AddTypes(v => v.FromBasic()
							.ToStringMethod.Set(o => o.Id)
							.Namespace.Set("Routine.Test.Module.Virtual")
							.Name.Set("VirtualType")
							.Methods.Add(o => o.Proxy<int>().TargetByParameter("i"))
							.Methods.Add(o => o.Virtual("VirtualService", (string str, int i) => string.Join(",", Enumerable.Range(0, i).Select(ix => str))))
						)
						.StaticInstances.Add(c => c.By(t => new VirtualObject("Instance", t as VirtualType)).When(t => t is VirtualType))
						.Use(p => p.ParseableValueTypePattern())
						.Use(p => p.EnumPattern(false))
						.Use(p => p.SingletonPattern(container, "Instance"))
						.Use(p => p.AutoMarkWithAttributesPattern(o => o.GetType().Namespace != null && o.GetType().Namespace.StartsWith("Routine")))

						.TypeMarks.Add("Module", t => t is TypeInfo && container.TypeIsSingleton(t) && t.FullName.EndsWith("Module"))
						.TypeMarks.Add("Search", t => t.CanBe<IQuery>())
						.OperationMarks.Add("ParamOptions", o => o.HasNoParameters() && o.ReturnsCollection() && o.Name.Matches("GetAvailable.*sFor.*"))
						.OperationMarks.Add("Virtual", o => o is VirtualMethod || o is ProxyMethod)
						.ParameterMarks.Add("Interface", p => p.ParameterType.IsInterface)

						.Module.Set(c => c.By(t => t.Namespace.After("Module.").BeforeLast(".").Prepend("Test.")).When(t => t.Namespace.StartsWith("Routine")))

						.Initializers.Add(c => c.PublicConstructors().When(t => t.IsValueType && t.Namespace != null && t.Namespace.StartsWith("Routine.Test.Module")))
						.Datas.Add(c => c.PublicProperties(m => !m.IsInherited(true, true) && !m.Returns<Guid>() && !m.Returns<TypedGuid>())
										   .When(t => t.Namespace != null && t.Namespace.StartsWith("Routine.Test.Module")))

						.Operations.Add(c => c.PublicMethods(o => !o.IsInherited(true, true) && o.Parameters.All(p => !p.ParameterType.Equals(type.of<Guid>()) && !p.ParameterType.Equals(type.of<TypedGuid>())))
											  .When(t => t.Namespace != null && t.Namespace.StartsWith("Routine.Test.Module")))
						//.Operations.Add(c => c.Build(o => o.Proxy<string>("Replace").TargetByParameter("str")).When(t => t.IsOptimized && !t.IsValueType && !t.IsInterface))
						//.Operations.Add(c => c.Build(o => o.Virtual("Concat", (string str1, string str2) => str1 + str2)).When(t => t.IsOptimized && !t.IsValueType && !t.IsInterface))

						.ValueExtractor.Set(c => c.ValueByProperty(m => m.Returns<string>("Title")))
						.ValueExtractor.Set(c => c.ValueByProperty(m => m.Returns<string>("Name")))
						.ValueExtractor.Set(c => c.Value(e => e.By(o => o.GetType().Name.BeforeLast("Module").SplitCamelCase().ToUpperInitial()))
												  .When(t => t is TypeInfo && container.TypeIsSingleton(t)))
						.ValueExtractor.Set(c => c.Value(e => e.By(o => string.Format("{0}", o))))

						.Use(p => p.FromEmpty()
							.IdExtractor.Set(c => c.Id(e => e.By(o => container.Resolve<ISession>().GetIdentifier(o).ToString()))
												   .When(t => t is TypeInfo && Orm.IsPersistent((TypeInfo)t)))
							.Locator.Set(c => c.Locator(l => l.SingleBy((t, id) => container.Resolve<ISession>().Get(((TypeInfo)t).GetActualType(), Guid.Parse(id))).AcceptNullResult(false))
											   .When(t => t is TypeInfo && Orm.IsPersistent((TypeInfo)t)))
							)
						;
			}

			private IServiceConfiguration ServiceConfiguration()
			{
				return BuildRoutine.ServiceConfig()
						.FromBasic()
						.RootPath.Set("Service")
						.AllowGet.Set(true, m => m.ObjectModel.Marks.Contains("Search"))
						.RequestHeaders.Add("language_code")
						.RequestHeaderProcessors.Add(c => c
							.For("language_code")
							.Do(languageCode =>
							{
								Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(languageCode ?? "en-US");
							}))
						.ResponseHeaders.Add("code", "message", "always-empty")
						.ResponseHeaderValue.Set("0", "code")
						.ResponseHeaderValue.Set("success", "message")
						.Use(p => p.ExceptionsWrappedAsUnhandledPattern())
						;
			}

			private IInterceptionConfiguration ServerInterceptionConfiguration()
			{
				return BuildRoutine.InterceptionConfig()
						.FromBasic()
						.Interceptors.Add(c => c.Interceptor(i => i.ByDecorating(() => container.BeginScope()).After(s => s.Dispose())))
						.Interceptors.Add(p => p.Interceptor(i => i.ByDecorating(() => container.Resolve<ISession>().BeginTransaction())
																   .Success(t => t.Commit())
																   .Fail(t => t.Rollback())))

						.ServiceInterceptors.Add(c => c.Interceptor(i => i.Before(ctx => Debug.WriteLine(string.Format("performing -> {0}", ctx.OperationName)))
																		  .Success(ctx => Debug.WriteLine(string.Format("\treturns -> {0}", ctx.Result)))
																		  .Fail(ctx => Debug.WriteLine(string.Format("\tthrows -> {0}", ctx.Exception)))
																		  .After(ctx => Debug.WriteLine(string.Format("end of {0}", ctx.OperationName)))))
						.ServiceInterceptors.Add(c => c.Interceptor(i => i.Before(() => { throw new Exception("Cannot call hidden service"); }))
													   .When(iom => iom.OperationModel.Marks.Contains("Hidden")))
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
						.LifestyleTransient()
					);
			}

			private AssemblyFilter BinDirectory { get { return new AssemblyFilter("bin"); } }

			private AssemblyFilter ModuleDirectory
			{
				get
				{
					return BinDirectory.FilterByName(n =>
						n.Name.StartsWith("Routine.Test.Module") &&
						!n.Name.EndsWith("Ui"));
				}
			}
			private AssemblyFilter UiDirectory
			{
				get
				{
					return BinDirectory.FilterByName(n =>
						n.Name.StartsWith("Routine.Test.Module") &&
						n.Name.EndsWith("Ui"));
				}
			}

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

			private IEnumerable<Assembly> ModuleAssemblies()
			{
				var result = ReflectionUtil.GetAssemblies(ModuleDirectory);

				var moduleAssemblies = result.ToList();
#if DEBUG
				Debug.WriteLine("assembly count: " + moduleAssemblies.Count());
				foreach (var item in moduleAssemblies)
				{
					Debug.WriteLine("module: " + item.FullName);
				}
#endif

				return moduleAssemblies;
			}

			private IEnumerable<Assembly> UiAssemblies()
			{
				var result = ReflectionUtil.GetAssemblies(UiDirectory);

				var uiAssemblies = result.ToList();
#if DEBUG
				Debug.WriteLine("assembly count: " + uiAssemblies.Count());
				foreach (var item in uiAssemblies)
				{
					Debug.WriteLine("ui: " + item.FullName);
				}
#endif

				return uiAssemblies;
			}

			private ISession OpenSession(IKernel kernel)
			{
				return kernel.Resolve<ISessionFactory>().OpenSession();
			}
		}
	}
}

