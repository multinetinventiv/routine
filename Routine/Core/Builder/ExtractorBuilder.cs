using System;
using Routine.Core.Extractor;

namespace Routine.Core.Builder
{
	public class ExtractorBuilder<TFrom, TData> 
	{
		internal ReferenceValueExtractor<TFrom, TData> ByReference(Func<TFrom, ISelector<TypeInfo, IMember>> selectorDelegate)
		{
			return new ReferenceValueExtractor<TFrom, TData>(selectorDelegate);
		}

		internal ReferenceValueExtractor<TFrom, TData> ByReference(ISelector<TypeInfo, IMember> selector)
		{
			return new ReferenceValueExtractor<TFrom, TData>(selector);
		}

		internal ConverterExtractor<TFrom, TData> ByConverting()
		{
			return new ConverterExtractor<TFrom, TData>();
		}

		public StaticExtractor<TFrom, TData> Always(TData result)
		{
			return new StaticExtractor<TFrom, TData>(result);
		}
			
		//facade
		public ConverterExtractor<TFrom, TData> ByConverting(Func<TFrom, TData> converterDelegate)
		{
			return ByConverting().Return(converterDelegate);
		}
	}
}
