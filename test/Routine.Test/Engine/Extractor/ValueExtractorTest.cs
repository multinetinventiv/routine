using System;
using NUnit.Framework;
using Routine.Core.Configuration.Convention;
using Routine.Engine;
using Routine.Engine.Extractor;

namespace Routine.Test.Engine.Extractor
{
    [TestFixture]
    public class ValueExtractorTest : ExtractorContract<IValueExtractor>
    {
        protected override IConvention<IType, IValueExtractor> CreateConventionByPublicMethod(Func<IMethod, bool> filter, Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate) =>
            BuildRoutine.Convention<IType, IValueExtractor>().ValueByPublicMethod(filter, configurationDelegate);

        protected override IConvention<IType, IValueExtractor> CreateConventionByDelegate(Func<object, string> extractorDelegate) =>
            BuildRoutine.Convention<IType, IValueExtractor>().Value(e => e.By(extractorDelegate));

        protected override string Extract(IValueExtractor extractor, object obj) =>
            extractor.GetValue(obj);
    }
}