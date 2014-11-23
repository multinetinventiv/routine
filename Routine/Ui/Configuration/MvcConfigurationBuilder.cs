namespace Routine.Ui.Configuration
{
	public class MvcConfigurationBuilder
	{
		public ConventionalMvcConfiguration FromScratch() { return FromScratch(ConventionalMvcConfiguration.DEFAULT_OBJECT_ID);}
		public ConventionalMvcConfiguration FromScratch(string defaultObjectId)
		{
			return new ConventionalMvcConfiguration(defaultObjectId);
		}

		public ConventionalMvcConfiguration FromBasic(){ return FromBasic(ConventionalMvcConfiguration.DEFAULT_OBJECT_ID);}
		public ConventionalMvcConfiguration FromBasic(string defaultObjectId)
		{
			return FromScratch(defaultObjectId)
					.NullDisplayValue.Set("-")
					.ViewNameSeparator.Set('-')
					.ListValueSeparator.Set(',')
					
					.ParameterDefault.OnFailReturn(null)
					.ParameterSearcher.OnFailReturn(null)

					.ViewName.OnFailReturn(string.Empty)

					.ViewRouteName.OnFailReturn("Get")
					.PerformRouteName.OnFailReturn("Perform")

					.OperationOrder.OnFailReturn(0)

					.MemberOrder.OnFailReturn(0)
					.SimpleMemberOrder.OnFailReturn(0)
					.TableMemberOrder.OnFailReturn(0)

					.OperationIsAvailable.OnFailReturn(true)
					.OperationIsRendered.OnFailReturn(true)
					.OperationIsSimple.OnFailReturn(false)

					.MemberIsRendered.OnFailReturn(true)
					.MemberIsSimple.OnFailReturn(true)
					.MemberIsTable.OnFailReturn(false)
					;
		}
	}
}

