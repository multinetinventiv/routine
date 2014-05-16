using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routine.Core;
using Routine.Core.Builder;
using Routine.Core.Configuration;
using Routine.Soa;
using Routine.Soa.Configuration;

namespace Routine
{
	public static class InterceptionPatterns
	{
		public static GenericInterceptionConfiguration FromEmpty(this PatternBuilder<GenericInterceptionConfiguration> source) { return new GenericInterceptionConfiguration(); }

		public static GenericInterceptionConfiguration CommonInterceptorPattern(
			this PatternBuilder<GenericInterceptionConfiguration> source, 
			Func<InterceptorBuilder<InterceptionContext>, IInterceptor<InterceptionContext>> interceptorBuilder)
		{
			return source.CommonInterceptorPattern(interceptorBuilder(BuildRoutine.Interceptor<InterceptionContext>()));
		}

		public static GenericInterceptionConfiguration CommonInterceptorPattern(this PatternBuilder<GenericInterceptionConfiguration> source, IInterceptor<InterceptionContext> interceptor)
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
