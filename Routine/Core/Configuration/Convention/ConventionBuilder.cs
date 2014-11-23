using System;

namespace Routine.Core.Configuration.Convention
{
	public class ConventionBuilder<TFrom, TData> 
	{
		internal DelegateBasedConvention<TFrom, TData> By()
		{
			return new DelegateBasedConvention<TFrom, TData>();
		}

		//facade
		public DelegateBasedConvention<TFrom, TData> By(Func<TFrom, TData> converterDelegate)
		{
			return By().Return(converterDelegate);
		}

		public DelegateBasedConvention<TFrom, TData> Constant(TData result)
		{
			return By(o => result);
		}
	}
}