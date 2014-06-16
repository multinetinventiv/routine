using System.Collections.Generic;
using Routine.Core;

namespace Routine.Api
{
	public class ParameterCodeModel : CodeModelBase
	{
		public ParameterCodeModel(IApiGenerationContext context)
			: base(context) { }

		private ParameterModel model;

		internal ParameterCodeModel With(ParameterModel model)
		{
			this.model = model;

			return this;
		}

		public string Id { get { return model.Id; } }
		public ObjectCodeModel Model { get { return CreateObject().With(model.ViewModelId, model.IsList); } }
		
		internal List<int> Groups { get { return model.Groups; } }

		public bool MarkedAs(string mark)
		{
			return model.Marks.Contains(mark);
		}
	}
}
