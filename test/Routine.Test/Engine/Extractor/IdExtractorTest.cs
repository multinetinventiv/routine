using System;
using NUnit.Framework;
using Routine.Core.Configuration.Convention;
using Routine.Engine;
using Routine.Engine.Extractor;

namespace Routine.Test.Engine.Extractor
{
	[TestFixture]
	public class IdExtractorTest : ExtractorContract<IIdExtractor>
	{
		protected override IConvention<IType, IIdExtractor> CreateConventionByPublicMethod(Func<IMethod, bool> filter, Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate)
		{
			return BuildRoutine.Convention<IType, IIdExtractor>().IdByPublicMethod(filter, configurationDelegate);
		}

		protected override IConvention<IType, IIdExtractor> CreateConventionByDelegate(Func<object, string> extractorDelegate)
		{
			return BuildRoutine.Convention<IType, IIdExtractor>().Id(e => e.By(extractorDelegate));
		}

		protected override string Extract(IIdExtractor extractor, object obj)
		{
			return extractor.GetId(obj);
		}
	}
}