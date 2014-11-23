using System;
using Routine.Engine.Extractor;

namespace Routine.Engine.Configuration
{
	public abstract class ExtractorBuilder
	{
		internal MemberValueExtractor ByMemberValue(IMember member)
		{
			return new MemberValueExtractor(member);
		}

		public DelegateBasedExtractor By(Func<object, string> converterDelegate)
		{
			return new DelegateBasedExtractor(converterDelegate);
		}

		//facade
		public DelegateBasedExtractor Constant(string value)
		{
			return By(o => value);
		}
	}

	public class IdExtractorBuilder : ExtractorBuilder { }
	public class ValueExtractorBuilder : ExtractorBuilder { }
}
