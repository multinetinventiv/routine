using System;

namespace Routine.Core.Configuration.Convention
{
	public abstract class ConventionBase<TFrom, TResult> : IConvention<TFrom, TResult>
	{
		private Func<TFrom, bool> whenDelegate;

		protected ConventionBase()
		{
			whenDelegate = o => true;
		}

		public IConvention<TFrom, TResult> WhenDefault() { return When(default(TFrom)); }
		public IConvention<TFrom, TResult> When(TFrom expected) { return When(o => Equals(o, expected)); }

		public ConventionBase<TFrom, TResult> When(Func<TFrom, bool> whenDelegate)
		{
			this.whenDelegate = this.whenDelegate.And(whenDelegate);

			return this;
		}

		protected virtual bool AppliesTo(TFrom obj)
		{
			return whenDelegate(obj);
		}

		private TResult SafeApply(TFrom obj)
		{
			if (!AppliesTo(obj)) { throw new ConfigurationException(obj); }

			return Apply(obj);
		}

		protected abstract TResult Apply(TFrom obj);

		#region IConvention implementation

		bool IConvention<TFrom, TResult>.AppliesTo(TFrom obj)
		{
			return AppliesTo(obj);
		}

		TResult IConvention<TFrom, TResult>.Apply(TFrom obj)
		{
			return SafeApply(obj);
		}

		#endregion
	}
}
