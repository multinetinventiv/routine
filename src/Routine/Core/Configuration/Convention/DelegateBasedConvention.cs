using System;

namespace Routine.Core.Configuration.Convention
{
    public class DelegateBasedConvention<TFrom, TResult> : ConventionBase<TFrom, TResult>
    {
        private Func<TFrom, TResult> converterDelegate;

        public DelegateBasedConvention() => Return(_ => default);

        public DelegateBasedConvention<TFrom, TResult> Return(Func<TFrom, TResult> converterDelegate) { this.converterDelegate = converterDelegate; return this; }

        protected override TResult Apply(TFrom obj) => converterDelegate(obj);
    }
}
