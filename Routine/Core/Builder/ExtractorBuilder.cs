using System;
using Routine.Core.Extractor;

namespace Routine.Core.Builder
{
	public class ExtractorBuilder<TFrom, TData> 
	{
		internal MemberValueExtractor<TFrom, TData> ByMember(Func<object, IMember> memberDelegate)
		{
			return new MemberValueExtractor<TFrom, TData>(memberDelegate);
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
