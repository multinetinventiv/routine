using System;
using Castle.Core;
using Castle.Windsor;
using Routine.Core.Builder;
using Routine.Core.Configuration;

namespace Routine.Test.Domain.Configuration
{
	internal static class WindsorPatterns
	{
		public static GenericCodingStyle SingletonPattern(this PatternBuilder<GenericCodingStyle> source, IWindsorContainer container, string singletonId)
		{
			return source
					.FromEmpty()
					.ExtractId.Done(e => e.Always(singletonId).WhenType(t => container.TypeIsSingleton(t)))
					.Locate.Done(l => l.Singleton(t => container.Resolve(t)).AcceptNullResult(false).WhenType(t => container.TypeIsSingleton(t)))
					.ExtractAvailableIds.Done(e => e.Always(singletonId).When(t => container.TypeIsSingleton(t)))
					;
		}

		public static bool TypeIsSingleton(this IWindsorContainer source, TypeInfo type)
		{
			return 
				source.Kernel.HasComponent(type.GetActualType()) &&
				source.Kernel.GetHandler(type.GetActualType()).ComponentModel.LifestyleType == LifestyleType.Singleton;
			
		}

		public static object Resolve(this IWindsorContainer container, TypeInfo typeInfo)
		{
			return container.Resolve(typeInfo.GetActualType());
		}
	}
}
