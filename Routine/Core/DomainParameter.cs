using System.Collections.Generic;
using System.Linq;
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
		public Marks Marks { get; private set; }
		public bool IsList{get;private set;}
		public string ViewModelId{get;private set;}
		public int Index{get{return parameter.Index;}}

		public DomainParameter For(DomainOperation domainOperation, IParameter parameter)
		{
			this.domainOperation = domainOperation;
			this.parameter = parameter;

			Id = parameter.Name;
			Marks = new Marks(ctx.CodingStyle.ParameterMarkSelector.Select(parameter));
			IsList = parameter.ParameterType.CanBeCollection();
			ViewModelId = ctx.CodingStyle.ModelIdSerializer.Serialize(IsList ? parameter.ParameterType.GetItemType() : parameter.ParameterType);

			return this;
		}

		public bool MarkedAs(string mark)
		{
			return Marks.Has(mark);
		}

		public ParameterModel GetModel()
		{
			return new ParameterModel {
				Id = Id,
				Marks = Marks.List,
				IsList = IsList,
				ViewModelId = ViewModelId
			};
		}
	}
}
