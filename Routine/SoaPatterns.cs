using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routine.Core;
using Routine.Core.Builder;
using Routine.Soa;
using Routine.Soa.Configuration;

namespace Routine
{
	public static class SoaPatterns
	{
		public static GenericSoaConfiguration FromEmpty(this PatternBuilder<GenericSoaConfiguration> source) { return new GenericSoaConfiguration(false); }

		public static GenericSoaConfiguration ExceptionsWrappedAsUnhandledPattern(this PatternBuilder<GenericSoaConfiguration> source)
		{
			return source.FromEmpty()
				.ExtractExceptionResult.Done(e => e.ByConverting(ex => new SoaExceptionResult(ex.GetType().FullName, ex.Message, false)))
			;
		}

		public static GenericSoaConfiguration CommonInterceptorPattern(
			this PatternBuilder<GenericSoaConfiguration> source, 
			Func<InterceptorBuilder<InterceptionContext>, IInterceptor<InterceptionContext>> interceptorBuilder)
		{
			return source.CommonInterceptorPattern(interceptorBuilder(BuildRoutine.Interceptor<InterceptionContext>()));
		}

		public static GenericSoaConfiguration CommonInterceptorPattern(this PatternBuilder<GenericSoaConfiguration> source,  IInterceptor<InterceptionContext> interceptor)
		{
			return source.FromEmpty()
				.InterceptGetApplicationModel.Done(i => i.Adapt(interceptor))
				.InterceptGetObjectModel.Done(i => i.Adapt(interceptor))
				.InterceptGetAvailableObjects.Done(i => i.Adapt(interceptor))
				.InterceptGet.Done(i => i.Adapt(interceptor))
				.InterceptGetValue.Done(i => i.Adapt(interceptor))
				.InterceptGetMember.Done(i => i.Adapt(interceptor))
				.InterceptGetOperation.Done(i => i.Adapt(interceptor))
				.InterceptPerformOperation.Done(i => i.Adapt(interceptor))
			;
		}
	}
}
