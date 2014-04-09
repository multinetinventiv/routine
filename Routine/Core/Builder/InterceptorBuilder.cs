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

		public AroundInterceptor<TContext> Do()
		{
			return new AroundInterceptor<TContext>();
		}

		//first level facade
		public AroundInterceptor<TContext> Before(Action beforeDelegate) { return Before(ctx => beforeDelegate()); }
		public AroundInterceptor<TContext> Before(Action<TContext> beforeDelegate)
		{
			return Do().Before(beforeDelegate);
		}

		public AroundInterceptor<TContext> Success(Action successDelegate) { return Success(ctx => successDelegate()); }
		public AroundInterceptor<TContext> Success(Action<TContext> successDelegate)
		{
			return Do().Success(successDelegate);
		}

		public AroundInterceptor<TContext> Fail(Action failDelegate) { return Fail(ctx => failDelegate()); }
		public AroundInterceptor<TContext> Fail(Action<TContext> failDelegate)
		{
			return Do().Fail(failDelegate);
		}

		public AroundInterceptor<TContext> After(Action afterDelegate) { return Fail(ctx => afterDelegate()); }
		public AroundInterceptor<TContext> After(Action<TContext> afterDelegate)
		{
			return Do().After(afterDelegate);
		}
	}
}
