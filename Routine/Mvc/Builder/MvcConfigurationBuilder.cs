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

					.MenuIds.OnFailReturn(new List<string>())

					.ParameterDefault.OnFailReturn(null)
					.ParameterOptions.OnFailReturn(new List<Robject>())

					.ViewName.OnFailReturn(string.Empty)

					.ViewRouteName.OnFailReturn("Get")
					.PerformRouteName.OnFailReturn("Perform")

					.OperationOrder.OnFailReturn(o => 0)

					.MemberOrder.OnFailReturn(m => 0)
					.SimpleMemberOrder.OnFailReturn(m => 0)
					.TableMemberOrder.OnFailReturn(m => 0)
					;
		}
	}
}

