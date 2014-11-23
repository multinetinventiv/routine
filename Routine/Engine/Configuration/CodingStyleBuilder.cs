using System.Collections.Generic;
using Routine.Engine.Configuration.Conventional;

namespace Routine.Engine.Configuration
{
	public class CodingStyleBuilder
	{
		public ConventionalCodingStyle FromScratch()
		{
			return new ConventionalCodingStyle()
				.TypeId.Set(s => s.Constant(Constants.NULL_MODEL_ID).When(t => t == null))
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

				.TypeId.Set(c => c.By(t => Constants.VOID_MODEL_ID).When(t => t != null && t.IsVoid))
				.TypeIsValue.Set(true, t => t.IsVoid)
			;
		}

		public ConventionalCodingStyle FromBasic()
		{
			return FromScratch()
				.Module.OnFailReturn(string.Empty)
				.TypeIsValue.OnFailReturn(false)

				.TypeIsView.Set(true, t => t.IsAbstract || t.IsInterface)
				.TypeIsView.OnFailReturn(false)

				.StaticInstances.OnFailReturn(new List<object>())
				.MemberFetchedEagerly.OnFailReturn(false)

				.IdExtractor.OnFailReturn(null)
				.ValueExtractor.OnFailReturn(null)
				.ObjectLocator.OnFailReturn(null)
			;
		}
	}
}
