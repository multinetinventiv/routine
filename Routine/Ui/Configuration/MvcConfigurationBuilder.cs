namespace Routine.Ui.Configuration
{
	public class MvcConfigurationBuilder
	{
		public ConventionalMvcConfiguration FromBasic()
		{
			return new ConventionalMvcConfiguration()
				.NullDisplayValue.Set("-")
				.ListValueSeparator.Set(',')
				.DefaultObjectId.Set("default")

				.StaticFileExtensions.Add("css", "js", "gif", "jpg", "png")

				.CachePolicyAction.Set(hcp => { })

				.ParameterDefault.SetDefault()
				.ParameterSearcher.SetDefault()

				.ObjectHasDetail.Set(c => c.By(ovm => ovm.HasMember || ovm.HasOperation))

				.OperationOrder.Set(0)
				.OptionOrder.Set(0)
				.MemberOrder.Set(0)

				.OperationIsAvailable.Set(true)
				.OperationIsRendered.Set(true)
				.OperationTypes.Set(OperationTypes.Page)
				.ConfirmationRequired.Set(true)

				.MemberIsRendered.Set(true)
				.MemberTypes.Set(MemberTypes.PageNameValue)

				.NextLayer()
			;
		}
	}
}

