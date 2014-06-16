using System.Collections.Generic;
using Routine.Core.Configuration;

namespace Routine.Core.Builder
{
	public class CodingStyleBuilder
	{
		public GenericCodingStyle FromScratch() { return new GenericCodingStyle(); }

		public GenericCodingStyle FromBasic()
		{
			return FromScratch()
					.SerializeModelId.Done(s => s.SerializeBy(t => Constants.VOID_MODEL_ID).SerializeWhen(t => t != null && t.IsVoid)
										.DeserializeBy(id => TypeInfo.Void()).DeserializeWhen(id => id == Constants.VOID_MODEL_ID))

					.ExtractModelModule.OnFailReturn(string.Empty)
					.ExtractModelIsValue.Add(e => e.Always(true).When(t => t.IsVoid))
								 .OnFailReturn(false)
					.ExtractModelIsView.Add(e => e.Always(true).When(t => t.IsAbstract || t.IsInterface))
								.OnFailReturn(false)

					.ExtractAvailableIds.OnFailReturn(new List<string>())
					.ExtractMemberFetchedEagerly.OnFailReturn(false)
					;
		}
	}
}
