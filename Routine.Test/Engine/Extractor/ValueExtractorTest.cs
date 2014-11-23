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
		protected override IConvention<IType, IValueExtractor> CreateConventionByPublicOperation(Func<IOperation, bool> operationFilter, Func<MemberValueExtractor, MemberValueExtractor> configurationDelegate)
		{
			return BuildRoutine.Convention<IType, IValueExtractor>().ValueByPublicOperation(operationFilter, configurationDelegate);
		}

		protected override IConvention<IType, IValueExtractor> CreateConventionByDelegate(Func<object, string> extractorDelegate)
		{
			return BuildRoutine.Convention<IType, IValueExtractor>().Value(e => e.By(extractorDelegate));
		}

		protected override string Extract(IValueExtractor extractor, object obj)
		{
			return extractor.GetValue(obj);
		}
	}
}