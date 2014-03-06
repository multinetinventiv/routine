using Routine.Api;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;
using System.Collections.Generic;

namespace Routine.Mvc
{
	public class OperationViewModel : ViewModelBase
	{
        public OperationViewModel(IMvcContext mvcContext)
			: base(mvcContext) {}

		private bool separator;
		private Roperation rop;

		internal OperationViewModel With(Roperation rop)
		{
			this.rop = rop;

			return this;
		}

		internal OperationViewModel Separator()
		{
			this.separator = true;

			return this;
		}

		private ObjectViewModel Target{get{return CreateObject().With(rop.Object);}}
		public bool IsAvailable {get{return rop.IsAvailable();}}
		public string Text{get{return MvcConfig.DisplayNameExtractor.Extract(rop.Id);}}
		public bool HasParameter{get{return rop.Parameters.Any();}}
		public bool IsSeparator{get{return separator;}}

		public List<ParameterViewModel> Parameters
		{
			get
			{
				return rop.Parameters.Select(p => CreateParameter().With(p)).ToList();
			}
		}

		public string PerformRouteName{get{return Target.PerformRouteName;}}
		public RouteValueDictionary RouteValues
		{
			get
			{
				var result = Target.RouteValues;
				result.Add("operationModelId", rop.Id);
				return result;
			}
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(OperationViewModel))
				return false;
			OperationViewModel other = (OperationViewModel)obj;
			return rop.Id == other.rop.Id;
		}
		

		public override int GetHashCode()
		{
			unchecked
			{
				return (Text != null ?Text.GetHashCode():0);
			}
		}
		
	}

	public static class UrlHelper_OperationViewModelExtensions
	{
		public static string Route(this UrlHelper source, OperationViewModel model)
		{
			return source.RouteUrl(model.PerformRouteName, model.RouteValues);
		}
	}
}
