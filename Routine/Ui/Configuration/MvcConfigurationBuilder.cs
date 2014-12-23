namespace Routine.Ui.Configuration
{
	public class MvcConfigurationBuilder
	{
		public ConventionalMvcConfiguration FromBasic(){ return FromBasic(ConventionalMvcConfiguration.DEFAULT_OBJECT_ID);}
		public ConventionalMvcConfiguration FromBasic(string defaultObjectId)
		{
			return new ConventionalMvcConfiguration(defaultObjectId)
				.NullDisplayValue.Set("-")
				.ViewNameSeparator.Set('-')
				.ListValueSeparator.Set(',')

				.ParameterDefault.SetDefault()
				.ParameterSearcher.SetDefault()

				.ViewName.Set(string.Empty)

				.ViewRouteName.Set("Get")
				.PerformRouteName.Set("Perform")

				.OperationOrder.Set(0)
				.OptionOrder.Set(0)
				.MemberOrder.Set(0)

				.OperationIsAvailable.Set(true)
				.OperationIsRendered.Set(true)
				.OperationTypes.Set(OperationTypes.Page)

				.MemberIsRendered.Set(true)
				.MemberTypes.Set(MemberTypes.PageNameValue)

				.NextLayer()
			;
		}
	}
}

