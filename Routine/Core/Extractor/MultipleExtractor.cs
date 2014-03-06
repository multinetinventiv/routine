using System;
using System.Collections.Generic;

namespace Routine.Core.Extractor
{
	public class MultipleExtractor<TConfigurator, TFrom, TResult> : IExtractor<TFrom, TResult>
	{
		private readonly TConfigurator configurator;

		private readonly List<IOptionalExtractor<TFrom, TResult>> extractors;
		private readonly string valueToExtract;

		private bool defaultIsSet;
		private TResult defaultResult;
		private Func<TFrom, CannotExtractException> exceptionDelegate;

		public MultipleExtractor(TConfigurator configurator, string valueToExtract)
		{
			this.valueToExtract = valueToExtract;
			this.configurator = configurator;

			this.extractors = new List<IOptionalExtractor<TFrom, TResult>>();

			OnFailThrow(o => new CannotExtractException(valueToExtract, o));
		}

		public TConfigurator OnFailReturn(TResult defaultResult) { defaultIsSet = true; this.defaultResult = defaultResult; return configurator; }

		public TConfigurator OnFailThrow(CannotExtractException exception) { return OnFailThrow(o => exception); }
		public TConfigurator OnFailThrow(Func<TFrom, CannotExtractException> exceptionDelegate) { defaultIsSet = false; this.exceptionDelegate = exceptionDelegate; return configurator; }

		public TConfigurator Done() { return configurator; }
		public TConfigurator Done(IOptionalExtractor<TFrom, TResult> extractor) { Add(extractor); return configurator; }

		public MultipleExtractor<TConfigurator, TFrom, TResult> Add(IOptionalExtractor<TFrom, TResult> extractor)
		{
			this.extractors.Add(extractor);

			return this;
		}

		public MultipleExtractor<TConfigurator, TFrom, TResult> Merge(MultipleExtractor<TConfigurator, TFrom, TResult> other)
		{
			extractors.AddRange(other.extractors);

			return this;
		}

		protected virtual TResult Extract(TFrom obj)
		{
			try
			{
				foreach(var extractor in extractors) 
				{
					TResult result;
					if(extractor.TryExtract(obj, out result))
					{
						return result;
					}
				}
			}
			catch(CannotExtractException) { throw; }
			catch(Exception ex) { throw new CannotExtractException(valueToExtract, obj, ex); }

			if(defaultIsSet)
			{
				return defaultResult;
			}

			throw exceptionDelegate(obj);
		}

		#region IExtractor implementation
		TResult IExtractor<TFrom, TResult>.Extract(TFrom obj)
		{
			return Extract(obj);
		}
		#endregion
	}
	
}
