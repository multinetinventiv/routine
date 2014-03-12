using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Routine.Api;
using Routine.Core;
using Routine.Core.Extractor;
using Routine.Core.Selector;
using Routine.Core.Service;

namespace Routine.Mvc.Configuration
{
	public class GenericMvcConfiguration : IMvcConfiguration
	{
		public const string DEFAULT_OBJECT_ID = "default";

		private string NullDisplayValue {get;set;}
		private char ViewNameSeparator {get;set;}
		private char ListValueSeparator{get;set;}
		public GenericMvcConfiguration DisplayValueForNullIs(string nullDisplayValue) {NullDisplayValue = nullDisplayValue; return this; }
		public GenericMvcConfiguration SeparateViewNamesBy(char viewNameSeparator) { ViewNameSeparator = viewNameSeparator; return this; }
		public GenericMvcConfiguration SeparateListValuesBy(char listValueSeparator) { ListValueSeparator = listValueSeparator; return this; }

		public MultipleExtractor<GenericMvcConfiguration, ObjectModel, string> IndexId{ get; private set;}
		public MultipleExtractor<GenericMvcConfiguration, ObjectModel, List<string>> MenuIds{ get; private set;}

		public MultipleExtractor<GenericMvcConfiguration, Rparameter, Rvariable> ParameterDefault{ get; private set;}
		public MultipleExtractor<GenericMvcConfiguration, Rparameter, List<Robject>> ParameterOptions{ get; private set;}

		public MultipleExtractor<GenericMvcConfiguration, ViewModelBase, string> ViewName{ get; private set;}
		public MultipleExtractor<GenericMvcConfiguration, string, string> DisplayName{ get; private set;}

		public MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, string> ViewRouteName{ get; private set;}
		public MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, string> PerformRouteName{ get; private set;}

		public MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, Func<OperationViewModel, int>> OperationOrder{ get; private set;}
		public MultipleSelector<GenericMvcConfiguration, ObjectViewModel, Func<OperationViewModel, bool>> OperationGroup{ get; private set;}

		public MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, Func<MemberViewModel, int>> MemberOrder{ get; private set;}
		public MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, Func<MemberViewModel, int>> SimpleMemberOrder{ get; private set;}
		public MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, Func<MemberViewModel, int>> TableMemberOrder{ get; private set;}

		public GenericMvcConfiguration() : this(true) {}
		public GenericMvcConfiguration(string defaultObjectId) : this(true, defaultObjectId) { }
		internal GenericMvcConfiguration(bool rootConfig) : this(rootConfig, DEFAULT_OBJECT_ID) { }
		internal GenericMvcConfiguration(bool rootConfig, string defaultObjectId)
		{
			IndexId = new MultipleExtractor<GenericMvcConfiguration, ObjectModel, string>(this, "IndexId");
			MenuIds = new MultipleExtractor<GenericMvcConfiguration, ObjectModel, List<string>>(this, "MenuIds");

			ParameterDefault = new MultipleExtractor<GenericMvcConfiguration, Rparameter, Rvariable>(this, "ParameterDefault");
			ParameterOptions = new MultipleExtractor<GenericMvcConfiguration, Rparameter, List<Robject>>(this, "ParameterOptions");

			ViewName = new MultipleExtractor<GenericMvcConfiguration, ViewModelBase, string>(this, "ViewName");
			DisplayName = new MultipleExtractor<GenericMvcConfiguration, string, string>(this, "DisplayName");

			ViewRouteName = new MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, string>(this, "ViewRouteName");
			PerformRouteName = new MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, string>(this, "PerformRouteName");

			OperationOrder = new MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, Func<OperationViewModel, int>>(this, "OperationOrder");
			OperationGroup = new MultipleSelector<GenericMvcConfiguration, ObjectViewModel, Func<OperationViewModel, bool>>(this);

			MemberOrder = new MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, Func<MemberViewModel, int>>(this, "MemberOrder");
			SimpleMemberOrder = new MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, Func<MemberViewModel, int>>(this, "SimpleMemberOrder");
			TableMemberOrder = new MultipleExtractor<GenericMvcConfiguration, ObjectViewModel, Func<MemberViewModel, int>>(this, "TableMemberOrder");

			if(rootConfig)
			{
				RegisterRoutes(defaultObjectId);
			}
		}

		public GenericMvcConfiguration Merge(GenericMvcConfiguration other)
		{
			IndexId.Merge(other.IndexId);
			MenuIds.Merge(other.MenuIds);

			ParameterDefault.Merge(other.ParameterDefault);
			ParameterOptions.Merge(other.ParameterOptions);

			ViewName.Merge(other.ViewName);
			DisplayName.Merge(other.DisplayName);

			ViewRouteName.Merge(other.ViewRouteName);
			PerformRouteName.Merge(other.PerformRouteName);

			OperationOrder.Merge(other.OperationOrder);
			OperationGroup.Merge(other.OperationGroup);

			MemberOrder.Merge(other.MemberOrder);
			SimpleMemberOrder.Merge(other.SimpleMemberOrder);
			TableMemberOrder.Merge(other.TableMemberOrder);

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

		IExtractor<ObjectModel, string> IMvcConfiguration.IndexIdExtractor{get{return IndexId;}}
		IExtractor<ObjectModel, List<string>> IMvcConfiguration.MenuIdsExtractor{get{return MenuIds;}}

		IExtractor<Rparameter, Rvariable> IMvcConfiguration.ParameterDefaultExtractor{get{return ParameterDefault;}}
		IExtractor<Rparameter, List<Robject>> IMvcConfiguration.ParameterOptionsExtractor{get{return ParameterOptions;}}

		IExtractor<ViewModelBase, string> IMvcConfiguration.ViewNameExtractor{get{return ViewName;}}
		IExtractor<string, string> IMvcConfiguration.DisplayNameExtractor{get{return DisplayName;}}

		IExtractor<ObjectViewModel, string> IMvcConfiguration.ViewRouteNameExtractor{get{return ViewRouteName;}}
		IExtractor<ObjectViewModel, string> IMvcConfiguration.PerformRouteNameExtractor{get{return PerformRouteName;}}

		IExtractor<ObjectViewModel, Func<OperationViewModel, int>> IMvcConfiguration.OperationOrderExtractor{get{return OperationOrder;}}
		ISelector<ObjectViewModel, Func<OperationViewModel, bool>> IMvcConfiguration.OperationGroupSelector{get{return OperationGroup;}}

		IExtractor<ObjectViewModel, Func<MemberViewModel, int>> IMvcConfiguration.MemberOrderExtractor{get{return MemberOrder;}}
		IExtractor<ObjectViewModel, Func<MemberViewModel, int>> IMvcConfiguration.SimpleMemberOrderExtractor{get{return SimpleMemberOrder;}}
		IExtractor<ObjectViewModel, Func<MemberViewModel, int>> IMvcConfiguration.TableMemberOrderExtractor{get{return TableMemberOrder;}}

		#endregion
	}
}

