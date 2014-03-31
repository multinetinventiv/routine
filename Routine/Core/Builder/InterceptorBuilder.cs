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

		public GenericInterceptor<TContext> Wrap(IInterceptor<InterceptionContext> childInterceptor)
		{
			return Do()
				.Before(ctx => childInterceptor.OnBefore(ctx))
				.Success(ctx => childInterceptor.OnSuccess(ctx))
				.Fail(ctx => childInterceptor.OnFail(ctx))
				.After(ctx => childInterceptor.OnAfter(ctx))
			;
		}

		//first level facade
		public GenericInterceptor<TContext> Before(Action beforeDelegate) { return Before(ctx => beforeDelegate()); }
		public GenericInterceptor<TContext> Before(Action<TContext> beforeDelegate)
		{
			return Do().Before(beforeDelegate);
		}

		public GenericInterceptor<TContext> Success(Action successDelegate) { return Success(ctx => successDelegate()); }
		public GenericInterceptor<TContext> Success(Action<TContext> successDelegate)
		{
			return Do().Success(successDelegate);
		}

		public GenericInterceptor<TContext> Fail(Action failDelegate) { return Fail(ctx => failDelegate()); }
		public GenericInterceptor<TContext> Fail(Action<TContext> failDelegate)
		{
			return Do().Fail(failDelegate);
		}

		public GenericInterceptor<TContext> After(Action afterDelegate) { return Fail(ctx => afterDelegate()); }
		public GenericInterceptor<TContext> After(Action<TContext> afterDelegate)
		{
			return Do().After(afterDelegate);
		}
	}
}
