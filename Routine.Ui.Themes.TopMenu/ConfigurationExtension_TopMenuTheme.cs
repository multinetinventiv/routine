using System;
using Routine.Ui;
using Routine.Ui.Configuration;

namespace Routine
{
	public static class ConfigurationExtension_TopMenuTheme
	{
		public static ConventionBasedMvcConfiguration TopMenuTheme(this MvcConfigurationBuilder source, Func<ObjectViewModel, bool> searchViewModelPredicate)
		{
			return source.FromBasic()
				.ThemeAssembly.Set(typeof(ConfigurationExtension_TopMenuTheme).Assembly)
				.ThemeNamespace.Set("Routine.Ui.Themes.TopMenu")
				.ViewNameSeparator.Set('.')
				.ViewName.Set("Search", vm => vm is ObjectViewModel && searchViewModelPredicate(((ObjectViewModel)vm)))
				.ViewName.Set(c => c.By(vm => vm.GetType().Name.Before("ViewModel")))
				;
		}
	}
}