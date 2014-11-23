using System;

namespace Routine.Engine.Extractor
{
	public class DelegateBasedExtractor : ExtractorBase
	{
		private readonly Func<object, string> extractorDelegate;

		public DelegateBasedExtractor(Func<object, string> extractorDelegate)
		{
			if (extractorDelegate == null) { throw new ArgumentNullException("extractorDelegate"); }

			this.extractorDelegate = extractorDelegate;
		}

		protected override string Extract(object obj)
		{
			return extractorDelegate(obj);
		}
	}
}