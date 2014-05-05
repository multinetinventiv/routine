using Routine.Core;

namespace Routine.Api.Generator
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
	}
}
