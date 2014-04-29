using System;
using Routine.Core;
using Routine.Core.Builder;
using Routine.Mvc.Configuration;

namespace Routine
{
	public static class MvcPatterns
	{
		public static GenericMvcConfiguration FromEmpty(this PatternBuilder<GenericMvcConfiguration> source) { return new GenericMvcConfiguration(false); }

		public static GenericMvcConfiguration CommonInterceptorPattern(
			this PatternBuilder<GenericMvcConfiguration> source,
			Func<InterceptorBuilder<InterceptionContext>, IInterceptor<InterceptionContext>> interceptorBuilder)
		{
			return source.CommonInterceptorPattern(interceptorBuilder(BuildRoutine.Interceptor<InterceptionContext>()));
		}

		public static GenericMvcConfiguration CommonInterceptorPattern(this PatternBuilder<GenericMvcConfiguration> source, IInterceptor<InterceptionContext> interceptor)
		{
			return source.FromEmpty()
				.InterceptPerform.Done(i => i.Adapt(interceptor))
				.InterceptGet.Done(i => i.Adapt(interceptor))
				.InterceptGetAs.Done(i => i.Adapt(interceptor))
			;
		}
	}
}

