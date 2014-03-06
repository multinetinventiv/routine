using System;

namespace Routine.Core.Extractor
{
	public abstract class BaseOptionalExtractor<TConcrete, TFrom, TResult> : IOptionalExtractor<TFrom, TResult>
		where TConcrete : BaseOptionalExtractor<TConcrete, TFrom, TResult>
	{
		private Func<TFrom, bool> whenDelegate;

		protected BaseOptionalExtractor()
		{
			When(o => true);
		}

		public TConcrete WhenNull() { return When(o => o == null); }
		public TConcrete When(Func<TFrom, bool> whenDelegate) {this.whenDelegate = whenDelegate; return (TConcrete)this;}

		protected virtual bool CanExtract(TFrom obj) 
		{
			return whenDelegate(obj);
		}

		private TResult SafeExtract(TFrom obj)
		{
			if(!CanExtract(obj)) {throw new CannotExtractException();}

			return Extract(obj);
		}

		private bool TryExtract(TFrom obj, out TResult result)
		{
			if(!CanExtract(obj))
			{
				result = default(TResult);
				return false;
			}

			result = Extract(obj);
			return true;
		}

		protected abstract TResult Extract(TFrom obj);

		#region IOptionalExtractor implementation

		bool IOptionalExtractor<TFrom, TResult>.CanExtract(TFrom obj)
		{
			return CanExtract(obj);
		}

		bool IOptionalExtractor<TFrom, TResult>.TryExtract(TFrom obj, out TResult result)
		{
			return TryExtract(obj, out result);
		}
		#endregion
		
		#region IExtractor implementation

		TResult IExtractor<TFrom, TResult>.Extract(TFrom obj)
		{
			return SafeExtract(obj);
		}

		#endregion
	}
}
