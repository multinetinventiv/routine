using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Castle.Core.Internal;
using Castle.Core.Logging;
using Castle.Facilities.FactorySupport;
using Castle.Facilities.Logging;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NHibernate;
using Routine.Core;
using Routine.Mvc;
using Routine.Soa;
using Routine.Test.Common.Domain;
using Routine.Test.Common.Domain.NHibernate;
using Routine.Test.Common.Domain.Windsor;

namespace Routine.Test.Common.Configuration
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
					Component.For<IMvcContext>().Instance(BuildRoutine.Context().AsMvcApplication(MvcConfiguration(), CodingStyle())).LifestyleSingleton(),
					Component.For<MvcController>().ImplementedBy<MvcController>().LifestylePerWebRequest()
				);
				
				Modules();
				DataAccess();
			}

			public void SoaApplication()
			{
				container.Register(
					Component.For<ISoaContext>().Instance(BuildRoutine.Context().AsSoaApplication(SoaConfiguration(), CodingStyle())).LifestyleSingleton(),
					Component.For<SoaController>().ImplementedBy<SoaController>().LifestylePerWebRequest()
				);
				
				Modules();
				DataAccess();
			}

			private ICodingStyle CodingStyle()
			{
				return BuildRoutine.CodingStyle()
						.FromBasic()
						.Use(p => p.NullPattern("_null"))
						.Use(p => p.ParseableValueTypePattern(":"))
						.Use(p => p.EnumPattern())
						.Use(p => p.CommonDomainTypeRootNamespacePattern("Routine.Test.Module"))
						.Use(p => p.SingletonPattern(container, "Instance"))
						.Use(p => p.AutoMarkWithAttributesPattern())

						.SelectModelMarks.Done(s => s.Always("Transactional").When(t => !t.Name.EndsWith("Search")))

						.ExtractModelModule.Done(e => e.ByConverting(t => t.Namespace.After("Module.").BeforeLast("."))
												  .When(t => t.IsDomainType))
						.SelectMembers.Done(s => s.ByPublicProperties(p => p.IsWithinRootNamespace(true) && p.IsPubliclyReadable && !p.IsIndexer && !p.Returns<Guid>())
												  .When(t => t.IsDomainType))

						.SelectOperations.Done(s => s.ByPublicMethods(m => m.IsWithinRootNamespace(true) && m.GetParameters().All(p => p.ParameterType != type.of<Guid>()))
													 .When(t => t.IsDomainType))

						.ExtractMemberIsHeavy.Done(e => e.ByConverting(m => m.ReturnsCollection()))
						.ExtractOperationIsHeavy.Done(e => e.ByConverting(o => o.HasParameters()))
						.ExtractDisplayValue.Add(e => e.ByPublicProperty(p => p.Returns<string>("Title")))
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

						.Use(p => p.FromEmpty()
							.ExtractOperationIsAvailable.Done(e => e.ByPublicMethod((obj, op) => m => m.HasNoParameters() && m.Returns<bool>("Can" + op.Name)))
							.SelectOperations.Exclude.Done(o => o.HasNoParameters() && o.Returns<bool>() && o.Name.StartsWith("Can")))

						.ExtractOperationIsAvailable.Done(e => e.ByPublicProperty(p => p.Returns<bool>("Active"))
																.When((obj, op) => op.Name != "Activate"))
						;
			}

			private ISoaConfiguration SoaConfiguration()
			{
				return BuildRoutine.SoaConfig()
						.FromBasic()
						.Use(p => p.ExceptionsWrappedAsUnhandledPattern())
						.Use(p => p.CommonInterceptorPattern(i => i.ByDecorating(() => container.BeginScope()).After(s => s.Dispose())))
						.InterceptPerformOperation
							.Add(i => i.Do()
									.Before(ctx => Debug.WriteLine(string.Format("performing -> {0}", ctx.OperationModelId)))
									.Success(ctx => Debug.WriteLine(string.Format("\treturns -> {0}", ctx.Result)))
									.Fail(ctx => Debug.WriteLine(string.Format("\tthrows -> {0}", ctx.Exception)))
									.After(ctx => Debug.WriteLine(string.Format("end of {0}", ctx.OperationModelId))))
							.Add(i => i.Before(() => { throw new Exception("Cannot call hidden service"); })
									   .When(ctx => ctx.TargetOperation.MarkedAs("Hidden")))
							.Done(i => i.ByDecorating(ctx => container.Resolve<ISession>().BeginTransaction())
									.Success(t => t.Commit())
									.Fail(t => t.Rollback())
									.When(ctx => ctx.TargetType.MarkedAs("Transactional") ||
												ctx.TargetOperation.MarkedAs("Transactional")))
						;
			}

			private IMvcConfiguration MvcConfiguration()
			{
				return BuildRoutine.MvcConfig()
						.FromBasic("Instance")

						.InterceptPerform
							.Add(i => i.ByDecorating(() => container.BeginScope()).After(s => s.Dispose()))
							.Add(i => i.Do()
											.Before(ctx => Debug.WriteLine(string.Format("performing -> {0}", ctx.OperationModelId)))
											.Success(ctx => Debug.WriteLine(string.Format("\treturns -> {0}", ctx.Result)))
											.Fail(ctx => Debug.WriteLine(string.Format("\tthrows -> {0}", ctx.Exception)))
											.After(ctx => Debug.WriteLine(string.Format("end of {0}", ctx.OperationModelId))))
							.Done(i => i.ByDecorating(() => container.Resolve<ISession>().BeginTransaction())
										.Success(t => t.Commit())
										.Fail(t => t.Rollback())
										.When(ctx => ctx.Target.MarkedAs("Transactional")))

						.ExtractIndexId.Done(e => e.Always("Instance").When(om => !om.IsViewModel && om.Id.EndsWith("Module")))
						.ExtractMenuIds
							.Add(e => e.Always("Instance").When(om => !om.IsViewModel && om.Id.EndsWith("Search")))
							.Done(e => e.Always("Instance").When(om => !om.IsViewModel && om.Id.EndsWith("Module")))

						.ExtractViewName
							.Add(e => e.Always("Search").When(vmb => vmb is ObjectViewModel && ((ObjectViewModel)vmb).ViewModelId.EndsWith("Search")))
							.Done(e => e.ByConverting(vmb => vmb.GetType().Name.Before("ViewModel")))

						.SelectOperationGroupFunctions.Done(s => s.Always(o => !o.HasParameter))

						.ExtractParameterDefault.Done(e => e.ByConverting(p => p.Operation.Object[p.Id.ToUpperInitial()].GetValue())
													 .When(p => p.Operation.Id.Matches("Update.*") && p.Operation.Object.Members.Any(m => m.Id == p.Id.ToUpperInitial())))
						.SelectOperationGroupFunctions.Done(s => s.Always(o => o.Text.Matches("Update.*")))

						.ExtractParameterOptions
							.Add(e => e.ByConverting(p => p.Operation.Object.Application.Get("Instance", p.ViewModelId + "Search").Perform("All").List)
									   .When(p => p.Operation.Object.Application.ObjectModels.Any(m => m.Id == p.ViewModelId + "Search")))
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
				return (type.Name.EndsWith("Search") || type.Name.EndsWith("Module")) && 
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
				Console.WriteLine("assembly count: " + result.Count());
				foreach(var item in result)
				{
					Console.WriteLine("module: " + item.FullName);
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

