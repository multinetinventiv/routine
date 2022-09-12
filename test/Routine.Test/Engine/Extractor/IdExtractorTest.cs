using NUnit.Framework;
using Routine.Core.Configuration.Convention;
using Routine.Engine.Extractor;
using Routine.Engine;
using System;

namespace Routine.Test.Engine.Extractor;

[TestFixture]
public class IdExtractorTest : ExtractorContract<IIdExtractor>
{
    protected override IConvention<IType, IIdExtractor> CreateConventionByPublicMethod(Func<IMethod, bool> filter, Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate) =>
        BuildRoutine.Convention<IType, IIdExtractor>().IdByPublicMethod(filter, configurationDelegate);

    protected override IConvention<IType, IIdExtractor> CreateConventionByDelegate(Func<object, string> extractorDelegate) =>
        BuildRoutine.Convention<IType, IIdExtractor>().Id(e => e.By(extractorDelegate));

    protected override string Extract(IIdExtractor extractor, object obj) =>
        extractor.GetId(obj);
}
