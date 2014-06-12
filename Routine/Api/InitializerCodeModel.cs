using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routine.Core;

namespace Routine.Api
{
	public class InitializerCodeModel : CodeModelBase
	{
		public InitializerCodeModel(IApiGenerationContext context)
			: base(context) { }

		private InitializerModel model;

		internal InitializerCodeModel With(InitializerModel model)
		{
			this.model = model;

			return this;
		}

		public List<ParameterCodeModel> Parameters { get { return model.Parameters.Select(p => CreateParameter().With(p)).ToList(); } }
		public List<List<ParameterCodeModel>> Groups
		{
			get
			{
				var result = Enumerable.Range(0, model.GroupCount).Select(i => new List<ParameterCodeModel>()).ToList();

				foreach (var param in Parameters)
				{
					foreach (var group in param.Groups)
					{
						result[group].Add(param);
					}
				}

				return result;
			}
		}

		public bool MarkedAs(string mark)
		{
			return model.Marks.Contains(mark);
		}
	}
}
