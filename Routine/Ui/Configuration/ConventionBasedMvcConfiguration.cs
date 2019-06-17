using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Routine.Client;
using Routine.Core.Configuration;
using Routine.Interception;

namespace Routine.Ui.Configuration
{
	public class ConventionBasedMvcConfiguration : LayeredBase<ConventionBasedMvcConfiguration>, IMvcConfiguration
	{
		public SingleConfiguration<ConventionBasedMvcConfiguration, string> NullDisplayValue { get; private set; }
		public SingleConfiguration<ConventionBasedMvcConfiguration, char> ViewNameSeparator { get; private set; }
		public SingleConfiguration<ConventionBasedMvcConfiguration, char> ListValueSeparator { get; private set; }
		public SingleConfiguration<ConventionBasedMvcConfiguration, string> DefaultObjectId { get; private set; }
		public SingleConfiguration<ConventionBasedMvcConfiguration, string> RootPath { get; private set; }
		public SingleConfiguration<ConventionBasedMvcConfiguration, Assembly> ThemeAssembly { get; private set; }
		public SingleConfiguration<ConventionBasedMvcConfiguration, string> ThemeNamespace { get; private set; }
		public ListConfiguration<ConventionBasedMvcConfiguration, Assembly> UiAssemblies { get; private set; }
		public ListConfiguration<ConventionBasedMvcConfiguration, string> StaticFileExtensions { get; private set; }

		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, string, Action<HttpCachePolicy>> CachePolicyAction { get; private set; }

		public ConventionBasedListConfiguration<ConventionBasedMvcConfiguration, InterceptionTarget, IInterceptor<InterceptionContext>> Interceptors { get; private set; }

		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, Rtype, string> IndexId { get; private set; }
		public ConventionBasedListConfiguration<ConventionBasedMvcConfiguration, Rtype, string> MenuIds { get; private set; }

		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, ParameterViewModel, Rvariable> ParameterDefault { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, ParameterViewModel, List<Robject>> ParameterOptions { get; private set; }

		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, ViewModelBase, string> ViewName { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, string, string> DisplayName { get; private set; }

		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, ObjectViewModel, bool> ObjectHasDetail { get; private set; }

		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, TypedViewModel<OperationViewModel, OperationTypes>, int> OperationOrder { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, OptionViewModel, int> OptionOrder { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, TypedViewModel<DataViewModel, DataLocations>, int> DataOrder { get; private set; }

		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, OperationViewModel, bool> OperationIsAvailable { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, OperationViewModel, bool> OperationIsRendered { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, OperationViewModel, OperationTypes> OperationTypes { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, OperationViewModel, bool> ConfirmationRequired { get; private set; }

		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, DataViewModel, bool> DataIsRendered { get; private set; }
		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, DataViewModel, DataLocations> DataLocations { get; private set; }

		public ConventionBasedConfiguration<ConventionBasedMvcConfiguration, Rparameter, Robject> ParameterSearcher { get; private set; }

		public ConventionBasedMvcConfiguration()
		{
			NullDisplayValue = new SingleConfiguration<ConventionBasedMvcConfiguration, string>(this, "NullDisplayValue", true);
			ViewNameSeparator = new SingleConfiguration<ConventionBasedMvcConfiguration, char>(this, "ViewNameSeparator", true);
			ListValueSeparator = new SingleConfiguration<ConventionBasedMvcConfiguration, char>(this, "ListValueSeparator", true);
			DefaultObjectId = new SingleConfiguration<ConventionBasedMvcConfiguration, string>(this, "DefaultObjectId", true);
			RootPath = new SingleConfiguration<ConventionBasedMvcConfiguration, string>(this, "RootPath");
			ThemeAssembly = new SingleConfiguration<ConventionBasedMvcConfiguration, Assembly>(this, "ThemeAssembly", true);
			ThemeNamespace = new SingleConfiguration<ConventionBasedMvcConfiguration, string>(this, "ThemeNamespace", true);
			UiAssemblies = new ListConfiguration<ConventionBasedMvcConfiguration, Assembly>(this, "UiAssemblies");
			StaticFileExtensions = new ListConfiguration<ConventionBasedMvcConfiguration, string>(this, "StaticFileExtensions");

			CachePolicyAction = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, string, Action<HttpCachePolicy>>(this, "CachePolicyAction", true);

			Interceptors = new ConventionBasedListConfiguration<ConventionBasedMvcConfiguration, InterceptionTarget, IInterceptor<InterceptionContext>>(this, "Interceptors");

			IndexId = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, Rtype, string>(this, "IndexId", true);
			MenuIds = new ConventionBasedListConfiguration<ConventionBasedMvcConfiguration, Rtype, string>(this, "MenuIds");

			ParameterDefault = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, ParameterViewModel, Rvariable>(this, "ParameterDefault");
			ParameterOptions = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, ParameterViewModel, List<Robject>>(this, "ParameterOptions");
			ParameterSearcher = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, Rparameter, Robject>(this, "ParameterSearcher");

			ViewName = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, ViewModelBase, string>(this, "ViewName");
			DisplayName = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, string, string>(this, "DisplayName");

			ObjectHasDetail = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, ObjectViewModel, bool>(this, "ObjectHasDetail");

			OperationOrder = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, TypedViewModel<OperationViewModel, OperationTypes>, int>(this, "OperationOrder");
			OptionOrder = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, OptionViewModel, int>(this, "OptionOrder");
			DataOrder = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, TypedViewModel<DataViewModel, DataLocations>, int>(this, "DataOrder");

			OperationIsAvailable = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, OperationViewModel, bool>(this, "OperationIsAvailable");
			OperationIsRendered = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, OperationViewModel, bool>(this, "OperationIsRendered");
			OperationTypes = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, OperationViewModel, OperationTypes>(this, "OperationTypes");
			ConfirmationRequired = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, OperationViewModel, bool>(this, "ConfirmationRequired");

			DataIsRendered = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, DataViewModel, bool>(this, "DataIsRendered");
			DataLocations = new ConventionBasedConfiguration<ConventionBasedMvcConfiguration, DataViewModel, DataLocations>(this, "DataLocations");
		}

		public ConventionBasedMvcConfiguration Merge(ConventionBasedMvcConfiguration other)
		{
			UiAssemblies.Merge(other.UiAssemblies);
			StaticFileExtensions.Merge(other.StaticFileExtensions);

			CachePolicyAction.Merge(other.CachePolicyAction);

			Interceptors.Merge(other.Interceptors);

			IndexId.Merge(other.IndexId);
			MenuIds.Merge(other.MenuIds);

			ParameterDefault.Merge(other.ParameterDefault);
			ParameterOptions.Merge(other.ParameterOptions);
			ParameterSearcher.Merge(other.ParameterSearcher);

			ViewName.Merge(other.ViewName);
			DisplayName.Merge(other.DisplayName);

			ObjectHasDetail.Merge(other.ObjectHasDetail);

			OperationOrder.Merge(other.OperationOrder);
			OptionOrder.Merge(other.OptionOrder);
			DataOrder.Merge(other.DataOrder);

			OperationIsAvailable.Merge(other.OperationIsAvailable);
			OperationIsRendered.Merge(other.OperationIsRendered);
			OperationTypes.Merge(other.OperationTypes);
			ConfirmationRequired.Merge(other.ConfirmationRequired);

			DataIsRendered.Merge(other.DataIsRendered);
			DataLocations.Merge(other.DataLocations);

			return this;
		}

		#region IMvcConfiguration implementation

		string IMvcConfiguration.GetNullDisplayValue() { return NullDisplayValue.Get(); }
		char IMvcConfiguration.GetViewNameSeparator() { return ViewNameSeparator.Get(); }
		char IMvcConfiguration.GetListValueSeparator() { return ListValueSeparator.Get(); }
		string IMvcConfiguration.GetDefaultObjectId() { return DefaultObjectId.Get(); }
		string IMvcConfiguration.GetRootPath() { return RootPath.Get(); }
		List<Assembly> IMvcConfiguration.GetUiAssemblies() { return UiAssemblies.Get(); }
		Assembly IMvcConfiguration.GetThemeAssembly() { return ThemeAssembly.Get(); }
		string IMvcConfiguration.GetThemeNamespace() { return ThemeNamespace.Get(); }
		List<string> IMvcConfiguration.GetStaticFileExtensions() { return StaticFileExtensions.Get(); }

		Action<HttpCachePolicy> IMvcConfiguration.GetCachePolicyAction(string virtualPath) { return CachePolicyAction.Get(virtualPath); }

		IInterceptor<InterceptionContext> IMvcConfiguration.GetInterceptor(InterceptionTarget target) { return new ChainInterceptor<InterceptionContext>(Interceptors.Get(target)); }

		string IMvcConfiguration.GetIndexId(Rtype type) { return IndexId.Get(type); }
		List<string> IMvcConfiguration.GetMenuIds(Rtype type) { return MenuIds.Get(type); }

		Rvariable IMvcConfiguration.GetDefault(ParameterViewModel parameterViewModel) { return ParameterDefault.Get(parameterViewModel); }
		List<Robject> IMvcConfiguration.GetOptions(ParameterViewModel parameterViewModel) { return ParameterOptions.Get(parameterViewModel); }
		Robject IMvcConfiguration.GetSearcher(Rparameter rpar) { return ParameterSearcher.Get(rpar); }

		string IMvcConfiguration.GetViewName(ViewModelBase viewModel) { return ViewName.Get(viewModel); }
		string IMvcConfiguration.GetDisplayName(string key) { return DisplayName.Get(key); }

		bool IMvcConfiguration.GetHasDetail(ObjectViewModel objectViewModel) { return ObjectHasDetail.Get(objectViewModel); }

		int IMvcConfiguration.GetOrder(OperationViewModel operationViewModel, OperationTypes operationTypes) { return OperationOrder.Get(new TypedViewModel<OperationViewModel, OperationTypes>(operationViewModel, operationTypes)); }
		int IMvcConfiguration.GetOrder(OptionViewModel optionViewModel) { return OptionOrder.Get(optionViewModel); }
		int IMvcConfiguration.GetOrder(DataViewModel dataViewModel, DataLocations dataLocations) { return DataOrder.Get(new TypedViewModel<DataViewModel, DataLocations>(dataViewModel, dataLocations)); }

		bool IMvcConfiguration.IsAvailable(OperationViewModel operationViewModel) { return OperationIsAvailable.Get(operationViewModel); }
		bool IMvcConfiguration.IsRendered(OperationViewModel operationViewModel) { return OperationIsRendered.Get(operationViewModel); }
		OperationTypes IMvcConfiguration.GetOperationTypes(OperationViewModel operationViewModel) { return OperationTypes.Get(operationViewModel); }
		bool IMvcConfiguration.GetConfirmationRequired(OperationViewModel operationViewModel) { return ConfirmationRequired.Get(operationViewModel); }

		bool IMvcConfiguration.IsRendered(DataViewModel dataViewModel) { return DataIsRendered.Get(dataViewModel); }
		DataLocations IMvcConfiguration.GetDataLocations(DataViewModel dataViewModel) { return DataLocations.Get(dataViewModel); }

		#endregion

		public class TypedViewModel<TViewModel, TType> where TViewModel : ViewModelBase
		{
			public TViewModel ViewModel { get; private set; }
			public TType Type { get; private set; }

			public TypedViewModel(TViewModel viewModel, TType type)
			{
				ViewModel = viewModel;
				Type = type;
			}
		}
	}
}

