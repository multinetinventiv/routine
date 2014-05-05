using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Api.Generator
{
	public class OperationCodeModel : CodeModelBase
	{
		public OperationCodeModel(IApiGenerationContext context)
			: base(context) { }

		private OperationModel model;

		internal OperationCodeModel With(OperationModel model)
		{
			this.model = model;

			return this;
		}

		public string Id { get { return model.Id; } }
		public ObjectCodeModel ReturnModel 
		{ 
			get 
			{
				if (model.Result.IsVoid)
				{
					return CreateObject().Void();
				}

				return CreateObject().With(model.Result.ViewModelId, model.Result.IsList);
			} 
		}

		public List<ParameterCodeModel> Parameters { get { return model.Parameters.Select(p => CreateParameter().With(p)).ToList(); } }
	}
}
