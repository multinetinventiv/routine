using Routine.Engine.Extractor;
using System;

namespace Routine.Engine.Configuration
{
    public abstract class ExtractorBuilder
    {
        internal PropertyValueExtractor ByPropertyValue(IProperty property) => new(property);
        public DelegateBasedExtractor By(Func<object, string> converterDelegate) => new(converterDelegate);

        //facade
        public DelegateBasedExtractor Constant(string value) => By(_ => value);
    }

    public class IdExtractorBuilder : ExtractorBuilder { }
    public class ValueExtractorBuilder : ExtractorBuilder { }
}
