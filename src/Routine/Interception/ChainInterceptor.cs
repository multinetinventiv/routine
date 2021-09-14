using System;
using System.Collections.Generic;

namespace Routine.Interception
{
    public class ChainInterceptor<TContext> : IInterceptor<TContext>
        where TContext : InterceptionContext
    {
        private IChainLinkInterceptor<TContext> first;
        private IChainLinkInterceptor<TContext> last;

        public ChainInterceptor() : this(new List<IInterceptor<TContext>>()) { }
        public ChainInterceptor(IEnumerable<IInterceptor<TContext>> initialList)
        {
            foreach (var interceptor in initialList)
            {
                Add(interceptor);
            }
        }

        public void Add(IInterceptor<TContext> interceptor)
        {
            var newLink = new AdapterChainLinkInterceptor<TContext>(interceptor);

            if (first == null || last == null)
            {
                first = last = newLink;

                return;
            }

            last.Next = newLink;
            last = newLink;
        }

        public void Merge(ChainInterceptor<TContext> other)
        {
            if (other.first == null || other.last == null) { return; }

            if (first == null || last == null)
            {
                first = other.first;
                last = other.last;

                return;
            }

            last.Next = other.first;
            last = other.last;
        }

        private object Intercept(TContext context, Func<object> invocation) =>
            first == null
                ? invocation()
                : first.Intercept(context, invocation);

        #region IInterceptor<TContext> implementation

        object IInterceptor<TContext>.Intercept(TContext context, Func<object> invocation) => Intercept(context, invocation);

        #endregion
    }
}
