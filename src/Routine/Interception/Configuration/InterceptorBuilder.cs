using System;

namespace Routine.Interception.Configuration
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
			return ByDecorating(_ => beforeDelegate());
		}

		public AroundInterceptor<TContext> Do()
		{
			return new AroundInterceptor<TContext>();
		}

		//first level facade
		public AroundInterceptor<TContext> Before(Action beforeDelegate) { return Before(_ => beforeDelegate()); }
		public AroundInterceptor<TContext> Before(Action<TContext> beforeDelegate)
		{
			return Do().Before(beforeDelegate);
		}

		public AroundInterceptor<TContext> Success(Action successDelegate) { return Success(_ => successDelegate()); }
		public AroundInterceptor<TContext> Success(Action<TContext> successDelegate)
		{
			return Do().Success(successDelegate);
		}

		public AroundInterceptor<TContext> Fail(Action failDelegate) { return Fail(_ => failDelegate()); }
		public AroundInterceptor<TContext> Fail(Action<TContext> failDelegate)
		{
			return Do().Fail(failDelegate);
		}

		public AroundInterceptor<TContext> After(Action afterDelegate) { return Fail(_ => afterDelegate()); }
		public AroundInterceptor<TContext> After(Action<TContext> afterDelegate)
		{
			return Do().After(afterDelegate);
		}
	}
}
