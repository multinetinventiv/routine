using System.Collections.Generic;
using Routine.Engine.Configuration.Conventional;

namespace Routine.Engine.Configuration
{
	public class CodingStyleBuilder
	{
		public ConventionalCodingStyle FromBasic()
		{
			return new ConventionalCodingStyle()
				.MaxFetchDepth.Set(Constants.DEFAULT_MAX_FETCH_DEPTH)

				.Type.Set(c => c.By(o => o.GetTypeInfo()))

				.Module.Set(string.Empty)
				.TypeIsValue.Set(false)

				.TypeIsView.Set(true, t => t.IsAbstract || t.IsInterface)
				.TypeIsView.Set(false)

				.StaticInstances.Set(new List<object>())
				.MemberFetchedEagerly.Set(false)

				.IdExtractor.SetDefault()
				.ValueExtractor.SetDefault()
				.Locator.SetDefault()
				.Converter.SetDefault()

				.Override(cfg => cfg
					.TypeId.Set(Constants.NULL_MODEL_ID, t => t == null)
					.TypeIsValue.Set(true, t => t == null)
					.TypeIsView.Set(true, t => t == null)
					.Locator.Set(c => c.Locator(l => l.Constant(null)).WhenDefault())
					.IdExtractor.Set(c => c.Id(e => e.Constant(null)).WhenDefault())
					.Converter.Set(c => c.Converter(cv => cv.Constant(null)).WhenDefault())
					.ValueExtractor.Set(c => c.Value(e => e.Constant(string.Empty)).WhenDefault())
					.Module.Set(null, t => t == null)
					.TypeMarks.AddNoneWhen(t => t == null)
					.Initializers.AddNoneWhen(t => t == null)
					.Operations.AddNoneWhen(t => t == null)
					.Members.AddNoneWhen(t => t == null)
					.StaticInstances.Set(c => c.Constant(new List<object>()).WhenDefault())

					.TypeId.Set(c => c.By(t => Constants.VOID_MODEL_ID).When(t => t.IsVoid))
					.TypeIsValue.Set(true, t => t.IsVoid)
				)
					
				.Override(cfg => cfg
					.TypeIsValue.Set(true, t => t.CanBe<string>())
				
					.IdExtractor.Set(c => c.Id(e => e.By(o => o as string)).When(type.of<string>()))
					.Locator.Set(c => c.Locator(l => l.SingleBy(o => o)).When(type.of<string>()))

					.Members.AddNoneWhen(type.of<string>())
					.Operations.AddNoneWhen(type.of<string>())
				)

				.NextLayer()
			;
		}
	}
}
