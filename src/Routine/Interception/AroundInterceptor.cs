using System;

namespace Routine.Interception
{
    public class AroundInterceptor<TContext> : AroundInterceptorBase<AroundInterceptor<TContext>, TContext>
        where TContext : InterceptionContext
    {
        private static readonly Action<TContext> EMPTY = _ => { };
        private static Action<TContext> Wrap(Action @delegate) => @delegate == null ? null : _ => @delegate();

        private Action<TContext> beforeDelegate = EMPTY;
        private Action<TContext> successDelegate = EMPTY;
        private Action<TContext> failDelegate = EMPTY;
        private Action<TContext> afterDelegate = EMPTY;

        public AroundInterceptor<TContext> Before(Action beforeDelegate) => Before(Wrap(beforeDelegate));
        public AroundInterceptor<TContext> Before(Action<TContext> beforeDelegate) { this.beforeDelegate = beforeDelegate ?? this.beforeDelegate; return this; }

        public AroundInterceptor<TContext> Success(Action successDelegate) => Success(Wrap(successDelegate));
        public AroundInterceptor<TContext> Success(Action<TContext> successDelegate) { this.successDelegate = successDelegate ?? this.successDelegate; return this; }

        public AroundInterceptor<TContext> Fail(Action failDelegate) => Fail(Wrap(failDelegate));
        public AroundInterceptor<TContext> Fail(Action<TContext> failDelegate) { this.failDelegate = failDelegate ?? this.failDelegate; return this; }

        public AroundInterceptor<TContext> After(Action afterDelegate) => After(Wrap(afterDelegate));
        public AroundInterceptor<TContext> After(Action<TContext> afterDelegate) { this.afterDelegate = afterDelegate ?? this.afterDelegate; return this; }

        protected override void OnBefore(TContext context) => beforeDelegate(context);
        protected override void OnSuccess(TContext context) => successDelegate(context);
        protected override void OnFail(TContext context) => failDelegate(context);
        protected override void OnAfter(TContext context) => afterDelegate(context);
    }
}
