using System.Collections.Generic;
using Routine.Core.CodingStyle;

namespace Routine.Core.Builder
{
	public class CodingStyleBuilder
	{
		public GenericCodingStyle FromScratch() { return new GenericCodingStyle(); }

		public GenericCodingStyle FromBasic()
		{
			return FromScratch()
					.ModelId.Done(s => s.SerializeBy(t => GenericCodingStyle.VOID_MODEL_ID).SerializeWhen(t => t != null && t.IsVoid)
										.DeserializeBy(id => TypeInfo.Void()).DeserializeWhen(id => id == GenericCodingStyle.VOID_MODEL_ID))

					.Module.OnFailReturn(string.Empty)
					.ModelIsValue.Add(e => e.Always(true).When(t => t.IsVoid))
								 .OnFailReturn(false)
					.ModelIsView.Add(e => e.Always(true).When(t => t.IsAbstract || t.IsInterface))
								.OnFailReturn(false)

					.AvailableIds.OnFailReturn(new List<string>())

					.MemberIsHeavy.OnFailReturn(false)
					.OperationIsHeavy.OnFailReturn(false)

					.OperationIsAvailable.OnFailReturn(true)
					;
		}
	}
}
