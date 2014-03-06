using System;

namespace Routine.Core.Extractor
{
	public class ConverterExtractor<TFrom, TResult> : BaseOptionalExtractor<ConverterExtractor<TFrom, TResult>, TFrom, TResult>
	{
		private Func<TFrom, TResult> converterDelegate;

		public ConverterExtractor()
		{
			Return(o => default(TResult));
		}

		public ConverterExtractor<TFrom, TResult> Return(Func<TFrom, TResult> converterDelegate) { this.converterDelegate = converterDelegate; return this; }

		protected override TResult Extract(TFrom obj)
		{
			return converterDelegate(obj);
		}
	}
}
