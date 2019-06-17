namespace Routine.Ui.Configuration
{
	public class MvcConfigurationBuilder
	{
		public ConventionBasedMvcConfiguration FromBasic()
		{
			return new ConventionBasedMvcConfiguration()
				.NullDisplayValue.Set("-")
				.ListValueSeparator.Set(',')
				.DefaultObjectId.Set("default")

				.StaticFileExtensions.Add("css", "js", "gif", "jpg", "png")

				.CachePolicyAction.Set(hcp => { })

				.ParameterDefault.SetDefault()
				.ParameterSearcher.SetDefault()

				.ObjectHasDetail.Set(c => c.By(ovm => ovm.HasData || ovm.HasOperation))

				.OperationOrder.Set(0)
				.OptionOrder.Set(0)
				.DataOrder.Set(0)

				.OperationIsAvailable.Set(true)
				.OperationIsRendered.Set(true)
				.OperationTypes.Set(OperationTypes.Page)
				.ConfirmationRequired.Set(true)

				.DataIsRendered.Set(true)
				.DataLocations.Set(DataLocations.PageNameValue)

				.NextLayer()
			;
		}
	}
}

