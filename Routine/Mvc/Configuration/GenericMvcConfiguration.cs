using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Routine.Core;
using Routine.Core.Api;
using Routine.Core.Extractor;
using Routine.Core.Interceptor;
using Routine.Core.Selector;
using Routine.Mvc.Context;

namespace Routine.Mvc.Configuration
{
	public class GenericMvcConfiguration : IMvcConfiguration
	{
		public const string DEFAULT_OBJECT_ID = "default";

		private string NullDisplayValue { get; set; }
		public GenericMvcConfiguration DisplayValueForNullIs(string nullDisplayValue) { NullDisplayValue = nullDisplayValue; return this; }

		private char ViewNameSeparator { get; set; }
		public GenericMvcConfiguration SeparateViewNamesBy(char viewNameSeparator) { ViewNameSeparator = viewNameSeparator; return this; }

		private char ListValueSeparator{get;set;}
		public GenericMvcConfiguration SeparateListValuesBy(char listValueSeparator) { ListValueSeparator = listValueSeparator; return this; }

		public ChainInterceptor<GenericMvcConfiguration, PerformInterceptionContext> InterceptPerform { get; private set; }
		public ChainInterceptor<GenericMvcConfiguration, GetInterceptionContext> InterceptGet{ get; private set; }
		public ChainInterceptor<GenericMvcConfiguration, GetAsInterceptionContext> InterceptGetAs{ get; private set; }

		public MultipleExtractor<GenericMvcConfiguration, ObjectModel, string> ExtractIndexId{ get; private set;}
		public MultipleExtractor<GenericMvcConfiguration, ObjectModel, List<string>> ExtractMenuIds{ get; private set;}

		public MultipleExtractor<GenericMvcConfiguration, Rparameter, Rvariable> ExtractParameterDefault{ get; private set;}
		public MultipleExtractor<GenericMvcConfiguration, Rparameter, List<Robject>> ExtractParameterOptions{ get; private set;}

		public MultipleExtractor<GenericMvcConfiguration, ViewModelBase, string> ExtractViewName{ get; private set;}
		public MultipleExtractor<GenericMvcConfiguration, string, string> ExtractDisplayName{ get; private set;}

		public MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, string> ExtractViewRouteName{ get; private set;}
		public MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, string> ExtractPerformRouteName{ get; private set;}

		public MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, Func<OperationViewModel, int>> ExtractOperationOrder{ get; private set;}
		public MultipleSelector<GenericMvcConfiguration, ObjectViewModel, Func<OperationViewModel, bool>> SelectOperationGroupFunctions{ get; private set;}

		public MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, Func<MemberViewModel, int>> ExtractMemberOrder{ get; private set;}
		public MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, Func<MemberViewModel, int>> ExtractSimpleMemberOrder{ get; private set;}
		public MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, Func<MemberViewModel, int>> ExtractTableMemberOrder{ get; private set;}

		public MultipleExtractor<GenericMvcConfiguration, OperationViewModel, bool> ExtractOperationIsAvailable { get; private set; }

		public GenericMvcConfiguration() : this(true) {}
		public GenericMvcConfiguration(string defaultObjectId) : this(true, defaultObjectId) { }
		internal GenericMvcConfiguration(bool rootConfig) : this(rootConfig, DEFAULT_OBJECT_ID) { }
		internal GenericMvcConfiguration(bool rootConfig, string defaultObjectId)
		{
			InterceptPerform = new ChainInterceptor<GenericMvcConfiguration, PerformInterceptionContext>(this);
			InterceptGet = new ChainInterceptor<GenericMvcConfiguration, GetInterceptionContext>(this);
			InterceptGetAs = new ChainInterceptor<GenericMvcConfiguration, GetAsInterceptionContext>(this);

			ExtractIndexId = new MultipleExtractor<GenericMvcConfiguration, ObjectModel, string>(this, "IndexId");
			ExtractMenuIds = new MultipleExtractor<GenericMvcConfiguration, ObjectModel, List<string>>(this, "MenuIds");

			ExtractParameterDefault = new MultipleExtractor<GenericMvcConfiguration, Rparameter, Rvariable>(this, "ParameterDefault");
			ExtractParameterOptions = new MultipleExtractor<GenericMvcConfiguration, Rparameter, List<Robject>>(this, "ParameterOptions");

			ExtractViewName = new MultipleExtractor<GenericMvcConfiguration, ViewModelBase, string>(this, "ViewName");
			ExtractDisplayName = new MultipleExtractor<GenericMvcConfiguration, string, string>(this, "DisplayName");

			ExtractViewRouteName = new MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, string>(this, "ViewRouteName");
			ExtractPerformRouteName = new MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, string>(this, "PerformRouteName");

			ExtractOperationOrder = new MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, Func<OperationViewModel, int>>(this, "OperationOrder");
			SelectOperationGroupFunctions = new MultipleSelector<GenericMvcConfiguration, ObjectViewModel, Func<OperationViewModel, bool>>(this);

			ExtractMemberOrder = new MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, Func<MemberViewModel, int>>(this, "MemberOrder");
			ExtractSimpleMemberOrder = new MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, Func<MemberViewModel, int>>(this, "SimpleMemberOrder");
			ExtractTableMemberOrder = new MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, Func<MemberViewModel, int>>(this, "TableMemberOrder");

			ExtractOperationIsAvailable = new MultipleExtractor<GenericMvcConfiguration, OperationViewModel, bool>(this, "OperationIsAvailable");

			if(rootConfig)
			{
				RegisterRoutes(defaultObjectId);
			}
		}

		public GenericMvcConfiguration Merge(GenericMvcConfiguration other)
		{
			InterceptPerform.Merge(other.InterceptPerform);
			InterceptGet.Merge(other.InterceptGet);
			InterceptGetAs.Merge(other.InterceptGetAs);

			ExtractIndexId.Merge(other.ExtractIndexId);
			ExtractMenuIds.Merge(other.ExtractMenuIds);

			ExtractParameterDefault.Merge(other.ExtractParameterDefault);
			ExtractParameterOptions.Merge(other.ExtractParameterOptions);

			ExtractViewName.Merge(other.ExtractViewName);
			ExtractDisplayName.Merge(other.ExtractDisplayName);

			ExtractViewRouteName.Merge(other.ExtractViewRouteName);
			ExtractPerformRouteName.Merge(other.ExtractPerformRouteName);

			ExtractOperationOrder.Merge(other.ExtractOperationOrder);
			SelectOperationGroupFunctions.Merge(other.SelectOperationGroupFunctions);

			ExtractMemberOrder.Merge(other.ExtractMemberOrder);
			ExtractSimpleMemberOrder.Merge(other.ExtractSimpleMemberOrder);
			ExtractTableMemberOrder.Merge(other.ExtractTableMemberOrder);

			ExtractOperationIsAvailable.Merge(other.ExtractOperationIsAvailable);

			return this;
		}

		private void RegisterRoutes(string defaultObjectId)
		{
			RouteTable.Routes.MapRoute(
				"PerformAs",
				"{actualModelId}/{id}/As/{viewModelId}/Perform/{operationModelId}",
				new {controller="Mvc", action="PerformAs"}
			);

			RouteTable.Routes.MapRoute(
				"Perform",
				"{modelId}/{id}/Perform/{operationModelId}",
				new {controller="Mvc", action="Perform"}
			);

			RouteTable.Routes.MapRoute(
				"GetAs",
				"{actualModelId}/{id}/As/{viewModelId}",
				new { controller="Mvc", action = "GetAs", id = defaultObjectId}
			);

			RouteTable.Routes.MapRoute(
				"Get",
				"{modelId}/{id}",
				new { controller="Mvc", action = "Get", id = defaultObjectId}
			);

			RouteTable.Routes.MapRoute(
				"Index",
				"",
				new { controller="Mvc", action = "Index"}
			);
		}

		#region IMvcConfiguration implementation

		string IMvcConfiguration.NullDisplayValue{get{return NullDisplayValue;}}
		char IMvcConfiguration.ViewNameSeparator{get{return ViewNameSeparator;}}
		char IMvcConfiguration.ListValueSeparator{get{return ListValueSeparator;}}

		IInterceptor<PerformInterceptionContext> IMvcConfiguration.PerformInterceptor { get { return InterceptPerform; } }
		IInterceptor<GetInterceptionContext> IMvcConfiguration.GetInterceptor { get { return InterceptGet; } }
		IInterceptor<GetAsInterceptionContext> IMvcConfiguration.GetAsInterceptor { get { return InterceptGetAs; } }

		IExtractor<ObjectModel, string> IMvcConfiguration.IndexIdExtractor{get{return ExtractIndexId;}}
		IExtractor<ObjectModel, List<string>> IMvcConfiguration.MenuIdsExtractor{get{return ExtractMenuIds;}}

		IExtractor<Rparameter, Rvariable> IMvcConfiguration.ParameterDefaultExtractor{get{return ExtractParameterDefault;}}
		IExtractor<Rparameter, List<Robject>> IMvcConfiguration.ParameterOptionsExtractor{get{return ExtractParameterOptions;}}

		IExtractor<ViewModelBase, string> IMvcConfiguration.ViewNameExtractor{get{return ExtractViewName;}}
		IExtractor<string, string> IMvcConfiguration.DisplayNameExtractor{get{return ExtractDisplayName;}}

		IExtractor<ObjectViewModel, string> IMvcConfiguration.ViewRouteNameExtractor{get{return ExtractViewRouteName;}}
		IExtractor<ObjectViewModel, string> IMvcConfiguration.PerformRouteNameExtractor{get{return ExtractPerformRouteName;}}

		IExtractor<ObjectViewModel, Func<OperationViewModel, int>> IMvcConfiguration.OperationOrderExtractor{get{return ExtractOperationOrder;}}
		ISelector<ObjectViewModel, Func<OperationViewModel, bool>> IMvcConfiguration.OperationGroupSelector{get{return SelectOperationGroupFunctions;}}

		IExtractor<ObjectViewModel, Func<MemberViewModel, int>> IMvcConfiguration.MemberOrderExtractor{get{return ExtractMemberOrder;}}
		IExtractor<ObjectViewModel, Func<MemberViewModel, int>> IMvcConfiguration.SimpleMemberOrderExtractor{get{return ExtractSimpleMemberOrder;}}
		IExtractor<ObjectViewModel, Func<MemberViewModel, int>> IMvcConfiguration.TableMemberOrderExtractor{get{return ExtractTableMemberOrder;}}

		IExtractor<OperationViewModel, bool> IMvcConfiguration.OperationIsAvailableExtractor { get { return ExtractOperationIsAvailable; } }

		#endregion
	}
}

