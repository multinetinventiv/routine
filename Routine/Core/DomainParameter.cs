using System.Collections.Generic;
using Routine.Core.Service;

namespace Routine.Core
{
	public class DomainParameter
	{
		private readonly ICoreContext ctx;

		public DomainParameter(ICoreContext ctx)
		{
			this.ctx = ctx;
		}

		private DomainOperation domainOperation;
		private IParameter parameter;
		public TypeInfo Type{get{return parameter.ParameterType;}}

		public string Id { get; private set; }
		public List<string> Marks { get; private set; }
		public bool IsList{get;private set;}
		public string ViewModelId{get;private set;}
		public int Index{get{return parameter.Index;}}

		public DomainParameter For(DomainOperation domainOperation, IParameter parameter)
		{
			this.domainOperation = domainOperation;
			this.parameter = parameter;
			Marks = new List<string>();

			Id = parameter.Name;
			Marks.AddRange(ctx.CodingStyle.ParameterMarkSelector.Select(parameter));
			IsList = parameter.ParameterType.CanBeCollection();
			ViewModelId = ctx.CodingStyle.ModelIdSerializer.Serialize(IsList ? parameter.ParameterType.GetItemType() : parameter.ParameterType);

			return this;
		}

		public ParameterModel GetModel()
		{
			return new ParameterModel {
				Id = Id,
				Marks = new List<string>(Marks),
				IsList = IsList,
				ViewModelId = ViewModelId
			};
		}

		public ParameterData CreateData(object target)
		{
			return new ParameterData {
				ModelId = Id
			};
		}
	}
}
