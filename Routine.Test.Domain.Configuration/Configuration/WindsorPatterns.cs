using System.Collections.Generic;
using Castle.Core;
using Castle.Windsor;
using Routine.Core.Configuration;
using Routine.Engine;
using Routine.Engine.Configuration.Conventional;

namespace Routine.Test.Domain.Configuration
{
	internal static class WindsorPatterns
	{
		public static ConventionalCodingStyle SingletonPattern(this PatternBuilder<ConventionalCodingStyle> source, IWindsorContainer container, string singletonId)
		{
			return source
					.FromEmpty()
					.IdExtractor.Set(c => c.Id(e => e.Constant(singletonId))
										   .When(t => container.TypeIsSingleton(t)))
					.Locator.Set(c => c.Locator(l => l.Singleton(t => container.Resolve(t))
															.AcceptNullResult(false))
											 .When(t => container.TypeIsSingleton(t)))
					.StaticInstances.Set(c => c.By(t => container.Resolve(t))
											   .When(t => container.TypeIsSingleton(t)))
					;
		}

		public static bool TypeIsSingleton(this IWindsorContainer source, IType type)
		{
			return
				type is TypeInfo &&
				source.Kernel.HasComponent(((TypeInfo)type).GetActualType()) &&
				source.Kernel.GetHandler(((TypeInfo)type).GetActualType()).ComponentModel.LifestyleType == LifestyleType.Singleton;

		}

		public static object Resolve(this IWindsorContainer source, IType type)
		{
			return source.Resolve(((TypeInfo)type).GetActualType());
		}
	}
}
