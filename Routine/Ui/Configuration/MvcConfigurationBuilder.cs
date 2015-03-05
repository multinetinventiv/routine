using System;

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

		public ConventionalMvcConfiguration DefaultTheme(Func<ObjectViewModel, bool> searchViewModelPredicate)
		{
			return FromBasic()
				.ThemeAssembly.Set(typeof(MvcPatterns).Assembly)
				.ThemeNamespace.Set("Routine.Ui.Themes.Default")
				.ViewNameSeparator.Set('.')
				.ViewName.Set("Search", vm => vm is ObjectViewModel && searchViewModelPredicate(((ObjectViewModel)vm)))
				.ViewName.Set(c => c.By(vm => vm.GetType().Name.Before("ViewModel")))
				;
		}

		public ConventionalMvcConfiguration TopMenuTheme(Func<ObjectViewModel, bool> searchViewModelPredicate)
		{
			return FromBasic()
				.ThemeAssembly.Set(typeof(MvcPatterns).Assembly)
				.ThemeNamespace.Set("Routine.Ui.Themes.TopMenu")
				.ViewNameSeparator.Set('.')
				.ViewName.Set("Search", vm => vm is ObjectViewModel && searchViewModelPredicate(((ObjectViewModel)vm)))
				.ViewName.Set(c => c.By(vm => vm.GetType().Name.Before("ViewModel")))
				;
		}
	}
}

