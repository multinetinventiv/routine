using System;

namespace Routine.Engine.Extractor
{
	public class DelegateBasedExtractor : ExtractorBase
	{
		private readonly Func<object, string> extractorDelegate;

		public DelegateBasedExtractor(Func<object, string> extractorDelegate)
		{
            this.extractorDelegate = extractorDelegate ?? throw new ArgumentNullException(nameof(extractorDelegate));
		}

		protected override string Extract(object obj) => extractorDelegate(obj);
    }
}