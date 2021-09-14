using System;

namespace Routine.Interception.Configuration
{
	public class InterceptorBuilder<TContext>
		where TContext : InterceptionContext
	{
		public DecoratorInterceptor<TContext, TVariableType> ByDecorating<TVariableType>(Func<TContext, TVariableType> beforeDelegate) => new(beforeDelegate);
        public DecoratorInterceptor<TContext, TVariableType> ByDecorating<TVariableType>(Func<TVariableType> beforeDelegate) => ByDecorating(_ => beforeDelegate());

        public AroundInterceptor<TContext> Do() => new();

        //first level facade
		public AroundInterceptor<TContext> Before(Action beforeDelegate) => Before(_ => beforeDelegate());
        public AroundInterceptor<TContext> Before(Action<TContext> beforeDelegate) => Do().Before(beforeDelegate);

        public AroundInterceptor<TContext> Success(Action successDelegate) => Success(_ => successDelegate());
        public AroundInterceptor<TContext> Success(Action<TContext> successDelegate) => Do().Success(successDelegate);

        public AroundInterceptor<TContext> Fail(Action failDelegate) => Fail(_ => failDelegate());
        public AroundInterceptor<TContext> Fail(Action<TContext> failDelegate) => Do().Fail(failDelegate);

        public AroundInterceptor<TContext> After(Action afterDelegate) => Fail(_ => afterDelegate());
        public AroundInterceptor<TContext> After(Action<TContext> afterDelegate) => Do().After(afterDelegate);
    }
}
