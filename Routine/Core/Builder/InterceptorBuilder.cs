using System;
using Routine.Core.Interceptor;

namespace Routine.Core.Builder
{
	public class InterceptorBuilder<TContext>
		where TContext : InterceptionContext
	{
		public DecoratorInterceptor<TContext, TVariableType> ByDecorating<TVariableType>(Func<TContext, TVariableType> beforeDelegate)
		{
			return new DecoratorInterceptor<TContext, TVariableType>(beforeDelegate);
		}

		public DecoratorInterceptor<TContext, TVariableType> ByDecorating<TVariableType>(Func<TVariableType> beforeDelegate)
		{
			return ByDecorating(ctx => beforeDelegate());
		}

		public GenericInterceptor<TContext> Do()
		{
			return new GenericInterceptor<TContext>();
		}

		//first level facade
		public GenericInterceptor<TContext> Before(Action<TContext> beforeDelegate)
		{
			return Do().Before(beforeDelegate);
		}

		public GenericInterceptor<TContext> Before(Action beforeDelegate)
		{
			return Before(ctx => beforeDelegate());
		}

		public GenericInterceptor<TContext> After(Action<TContext> afterDelegate)
		{
			return Do().After(afterDelegate);
		}

		public GenericInterceptor<TContext> After(Action afterDelegate)
		{
			return After(ctx => afterDelegate());
		}

		public GenericInterceptor<TContext> Error(Action<TContext> errorDelegate)
		{
			return Do().Error(errorDelegate);
		}

		public GenericInterceptor<TContext> Error(Action errorDelegate)
		{
			return Error(ctx => errorDelegate());
		}
	}
}
