using System.Collections.Generic;
using Routine.Api;
using Routine.Mvc.Configuration;

namespace Routine.Mvc.Builder
{
	public class MvcConfigurationBuilder
	{
		public GenericMvcConfiguration FromScratch() { return FromScratch(GenericMvcConfiguration.DEFAULT_OBJECT_ID);}
		public GenericMvcConfiguration FromScratch(string defaultObjectId)
		{
			return new GenericMvcConfiguration(defaultObjectId);
		}

		public GenericMvcConfiguration FromBasic(){ return FromBasic(GenericMvcConfiguration.DEFAULT_OBJECT_ID);}
		public GenericMvcConfiguration FromBasic(string defaultObjectId)
		{
			return FromScratch(defaultObjectId)
					.DisplayValueForNullIs("-")
					.SeparateViewNamesBy('-')
					.SeparateListValuesBy(',')

					.ExtractMenuIds.OnFailReturn(new List<string>())

					.ExtractParameterDefault.OnFailReturn(null)
					.ExtractParameterOptions.OnFailReturn(new List<Robject>())

					.ExtractViewName.OnFailReturn(string.Empty)

					.ExtractViewRouteName.OnFailReturn("Get")
					.ExtractPerformRouteName.OnFailReturn("Perform")

					.ExtractOperationOrder.OnFailReturn(o => 0)

					.ExtractMemberOrder.OnFailReturn(m => 0)
					.ExtractSimpleMemberOrder.OnFailReturn(m => 0)
					.ExtractTableMemberOrder.OnFailReturn(m => 0)
					;
		}
	}
}

