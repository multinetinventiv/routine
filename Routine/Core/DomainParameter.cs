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
		public TypeInfo Type{get{return parameter.Type;}}

		public string Id{get;private set;}
		public bool IsList{get;private set;}
		public string ViewModelId{get;private set;}
		public int Index{get{return parameter.Index;}}

		public DomainParameter For(DomainOperation domainOperation, IParameter parameter)
		{
			this.domainOperation = domainOperation;
			this.parameter = parameter;

			Id = parameter.Name;
			IsList = parameter.Type.CanBeCollection();
			ViewModelId = ctx.CodingStyle.ModelIdSerializer.Serialize(IsList ?parameter.Type.GetItemType():parameter.Type);

			return this;
		}

		public ParameterModel GetModel()
		{
			return new ParameterModel {
				Id = Id,
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
