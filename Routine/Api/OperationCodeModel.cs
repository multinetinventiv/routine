using System.Collections.Generic;
using System.Linq;
using Routine.Client;

namespace Routine.Api
{
	public class OperationCodeModel
	{
		public Roperation Operation { get; private set; }
		public ObjectCodeModel ReturnModel { get; private set; }
		public List<ParameterCodeModel> Parameters { get; private set; }
		public List<List<ParameterCodeModel>> Groups { get; private set; }

		public OperationCodeModel(ApplicationCodeModel application, Roperation operation)
		{
			Operation = operation;

			if (operation.ResultIsVoid)
			{
				ReturnModel = application.GetVoidModel();
			}
			else
			{
				ReturnModel = application.GetModel(operation.ResultType, operation.ResultIsList);
			}

			Parameters = operation.Parameters.Select(p => new ParameterCodeModel(application, p)).ToList();
			Groups = Enumerable.Range(0, operation.Groups.Count).Select(i => new List<ParameterCodeModel>()).ToList();

			foreach (var param in Parameters)
			{
				foreach (var group in param.Groups)
				{
					Groups[group].Add(param);
				}
			}
		}

		public string Id { get { return Operation.Id; } }

		public bool MarkedAs(string mark)
		{
			return Operation.MarkedAs(mark);
		}
	}
}
