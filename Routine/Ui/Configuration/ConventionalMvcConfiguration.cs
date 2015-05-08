using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Routine.Client;
using Routine.Core.Configuration;
using Routine.Interception;

namespace Routine.Ui.Configuration
{
	public class ConventionalMvcConfiguration : LayeredBase<ConventionalMvcConfiguration>, IMvcConfiguration
	{
		public SingleConfiguration<ConventionalMvcConfiguration, string> NullDisplayValue { get; private set; }
		public SingleConfiguration<ConventionalMvcConfiguration, char> ViewNameSeparator { get; private set; }
		public SingleConfiguration<ConventionalMvcConfiguration, char> ListValueSeparator { get; private set; }
		public SingleConfiguration<ConventionalMvcConfiguration, string> DefaultObjectId { get; private set; }
		public SingleConfiguration<ConventionalMvcConfiguration, string> RootPath { get; private set; }
		public SingleConfiguration<ConventionalMvcConfiguration, Assembly> ThemeAssembly { get; private set; }
		public SingleConfiguration<ConventionalMvcConfiguration, string> ThemeNamespace { get; private set; }
		public ListConfiguration<ConventionalMvcConfiguration, Assembly> UiAssemblies { get; private set; }
		public ListConfiguration<ConventionalMvcConfiguration, string> StaticFileExtensions { get; private set; }

		public ConventionalConfiguration<ConventionalMvcConfiguration, string, Action<HttpCachePolicy>> CachePolicyAction { get; private set; }

		public ConventionalListConfiguration<ConventionalMvcConfiguration, InterceptionTarget, IInterceptor<InterceptionContext>> Interceptors { get; private set; }

		public ConventionalConfiguration<ConventionalMvcConfiguration, Rtype, string> IndexId { get; private set; }
		public ConventionalListConfiguration<ConventionalMvcConfiguration, Rtype, string> MenuIds { get; private set; }

		public ConventionalConfiguration<ConventionalMvcConfiguration, ParameterViewModel, Rvariable> ParameterDefault { get; private set; }
		public ConventionalConfiguration<ConventionalMvcConfiguration, ParameterViewModel, List<Robject>> ParameterOptions { get; private set; }

		public ConventionalConfiguration<ConventionalMvcConfiguration, ViewModelBase, string> ViewName { get; private set; }
		public ConventionalConfiguration<ConventionalMvcConfiguration, string, string> DisplayName { get; private set; }

		public ConventionalConfiguration<ConventionalMvcConfiguration, ObjectViewModel, bool> ObjectHasDetail { get; private set; }

		public ConventionalConfiguration<ConventionalMvcConfiguration, TypedViewModel<OperationViewModel, OperationTypes>, int> OperationOrder { get; private set; }
		public ConventionalConfiguration<ConventionalMvcConfiguration, OptionViewModel, int> OptionOrder { get; private set; }
		public ConventionalConfiguration<ConventionalMvcConfiguration, TypedViewModel<MemberViewModel, MemberTypes>, int> MemberOrder { get; private set; }

		public ConventionalConfiguration<ConventionalMvcConfiguration, OperationViewModel, bool> OperationIsAvailable { get; private set; }
		public ConventionalConfiguration<ConventionalMvcConfiguration, OperationViewModel, bool> OperationIsRendered { get; private set; }
		public ConventionalConfiguration<ConventionalMvcConfiguration, OperationViewModel, OperationTypes> OperationTypes { get; private set; }
		public ConventionalConfiguration<ConventionalMvcConfiguration, OperationViewModel, bool> ConfirmationRequired { get; private set; }

		public ConventionalConfiguration<ConventionalMvcConfiguration, MemberViewModel, bool> MemberIsRendered { get; private set; }
		public ConventionalConfiguration<ConventionalMvcConfiguration, MemberViewModel, MemberTypes> MemberTypes { get; private set; }

		public ConventionalConfiguration<ConventionalMvcConfiguration, Rparameter, Robject> ParameterSearcher { get; private set; }

		public ConventionalMvcConfiguration()
		{
			NullDisplayValue = new SingleConfiguration<ConventionalMvcConfiguration, string>(this, "NullDisplayValue", true);
			ViewNameSeparator = new SingleConfiguration<ConventionalMvcConfiguration, char>(this, "ViewNameSeparator", true);
			ListValueSeparator = new SingleConfiguration<ConventionalMvcConfiguration, char>(this, "ListValueSeparator", true);
			DefaultObjectId = new SingleConfiguration<ConventionalMvcConfiguration, string>(this, "DefaultObjectId", true);
			RootPath = new SingleConfiguration<ConventionalMvcConfiguration, string>(this, "RootPath");
			ThemeAssembly = new SingleConfiguration<ConventionalMvcConfiguration, Assembly>(this, "ThemeAssembly", true);
			ThemeNamespace = new SingleConfiguration<ConventionalMvcConfiguration, string>(this, "ThemeNamespace", true);
			UiAssemblies = new ListConfiguration<ConventionalMvcConfiguration, Assembly>(this, "UiAssemblies");
			StaticFileExtensions = new ListConfiguration<ConventionalMvcConfiguration, string>(this, "StaticFileExtensions");

			CachePolicyAction = new ConventionalConfiguration<ConventionalMvcConfiguration, string, Action<HttpCachePolicy>>(this, "CachePolicyAction", true);

			Interceptors = new ConventionalListConfiguration<ConventionalMvcConfiguration, InterceptionTarget, IInterceptor<InterceptionContext>>(this, "Interceptors");

			IndexId = new ConventionalConfiguration<ConventionalMvcConfiguration, Rtype, string>(this, "IndexId", true);
			MenuIds = new ConventionalListConfiguration<ConventionalMvcConfiguration, Rtype, string>(this, "MenuIds");

			ParameterDefault = new ConventionalConfiguration<ConventionalMvcConfiguration, ParameterViewModel, Rvariable>(this, "ParameterDefault");
			ParameterOptions = new ConventionalConfiguration<ConventionalMvcConfiguration, ParameterViewModel, List<Robject>>(this, "ParameterOptions");
			ParameterSearcher = new ConventionalConfiguration<ConventionalMvcConfiguration, Rparameter, Robject>(this, "ParameterSearcher");

			ViewName = new ConventionalConfiguration<ConventionalMvcConfiguration, ViewModelBase, string>(this, "ViewName");
			DisplayName = new ConventionalConfiguration<ConventionalMvcConfiguration, string, string>(this, "DisplayName");

			ObjectHasDetail = new ConventionalConfiguration<ConventionalMvcConfiguration, ObjectViewModel, bool>(this, "ObjectHasDetail");

			OperationOrder = new ConventionalConfiguration<ConventionalMvcConfiguration, TypedViewModel<OperationViewModel, OperationTypes>, int>(this, "OperationOrder");
			OptionOrder = new ConventionalConfiguration<ConventionalMvcConfiguration, OptionViewModel, int>(this, "OptionOrder");
			MemberOrder = new ConventionalConfiguration<ConventionalMvcConfiguration, TypedViewModel<MemberViewModel, MemberTypes>, int>(this, "MemberOrder");

			OperationIsAvailable = new ConventionalConfiguration<ConventionalMvcConfiguration, OperationViewModel, bool>(this, "OperationIsAvailable");
			OperationIsRendered = new ConventionalConfiguration<ConventionalMvcConfiguration, OperationViewModel, bool>(this, "OperationIsRendered");
			OperationTypes = new ConventionalConfiguration<ConventionalMvcConfiguration, OperationViewModel, OperationTypes>(this, "OperationTypes");
			ConfirmationRequired = new ConventionalConfiguration<ConventionalMvcConfiguration, OperationViewModel, bool>(this, "ConfirmationRequired");

			MemberIsRendered = new ConventionalConfiguration<ConventionalMvcConfiguration, MemberViewModel, bool>(this, "MemberIsRendered");
			MemberTypes = new ConventionalConfiguration<ConventionalMvcConfiguration, MemberViewModel, MemberTypes>(this, "MemberTypes");
		}

		public ConventionalMvcConfiguration Merge(ConventionalMvcConfiguration other)
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
			MemberOrder.Merge(other.MemberOrder);

			OperationIsAvailable.Merge(other.OperationIsAvailable);
			OperationIsRendered.Merge(other.OperationIsRendered);
			OperationTypes.Merge(other.OperationTypes);
			ConfirmationRequired.Merge(other.ConfirmationRequired);

			MemberIsRendered.Merge(other.MemberIsRendered);
			MemberTypes.Merge(other.MemberTypes);

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
		int IMvcConfiguration.GetOrder(MemberViewModel memberViewModel, MemberTypes memberTypes) { return MemberOrder.Get(new TypedViewModel<MemberViewModel, MemberTypes>(memberViewModel, memberTypes)); }

		bool IMvcConfiguration.IsAvailable(OperationViewModel operationViewModel) { return OperationIsAvailable.Get(operationViewModel); }
		bool IMvcConfiguration.IsRendered(OperationViewModel operationViewModel) { return OperationIsRendered.Get(operationViewModel); }
		OperationTypes IMvcConfiguration.GetOperationTypes(OperationViewModel operationViewModel) { return OperationTypes.Get(operationViewModel); }
		bool IMvcConfiguration.GetConfirmationRequired(OperationViewModel operationViewModel) { return ConfirmationRequired.Get(operationViewModel); }

		bool IMvcConfiguration.IsRendered(MemberViewModel memberViewModel) { return MemberIsRendered.Get(memberViewModel); }
		MemberTypes IMvcConfiguration.GetMemberTypes(MemberViewModel memberViewModel) { return MemberTypes.Get(memberViewModel); }

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

