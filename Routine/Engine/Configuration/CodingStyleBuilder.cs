using System.Collections.Generic;
using Routine.Engine.Configuration.Conventional;

namespace Routine.Engine.Configuration
{
	public class CodingStyleBuilder
	{
		public ConventionalCodingStyle FromBasic()
		{
			return new ConventionalCodingStyle()
				.Type.Set(c => c.By(o => o.GetTypeInfo()))

				.Module.Set(string.Empty)
				.TypeIsValue.Set(false)

				.TypeIsView.Set(true, t => t.IsAbstract || t.IsInterface)
				.TypeIsView.Set(false)

				.StaticInstances.Set(new List<object>())
				.MemberFetchedEagerly.Set(false)

				.IdExtractor.SetDefault()
				.ValueExtractor.SetDefault()
				.ObjectLocator.SetDefault()

				.Override(cfg => cfg
					.TypeId.Set(Constants.NULL_MODEL_ID, t => t == null)
					.TypeIsValue.Set(true, t => t == null)
					.TypeIsView.Set(true, t => t == null)
					.ObjectLocator.Set(c => c.Locator(l => l.Constant(null)).When(t => t == null))
					.IdExtractor.Set(c => c.Id(e => e.Constant(null)).When(t => t == null))
					.ValueExtractor.Set(c => c.Value(e => e.Constant(string.Empty)).When(t => t == null))
					.Module.Set(null, t => t == null)
					.TypeMarks.AddNoneWhen(t => t == null)
					.Initializers.AddNoneWhen(t => t == null)
					.Operations.AddNoneWhen(t => t == null)
					.Members.AddNoneWhen(t => t == null)
					.StaticInstances.Set(c => c.Constant(null).When(t => t == null))

					.TypeId.Set(c => c.By(t => Constants.VOID_MODEL_ID).When(t => t.IsVoid))
					.TypeIsValue.Set(true, t => t.IsVoid))

				.NextLayer()
			;
		}
	}
}
