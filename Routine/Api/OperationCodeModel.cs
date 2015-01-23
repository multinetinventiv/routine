using System.Collections.Generic;
using System.Linq;
using Routine.Client;

namespace Routine.Api
{
	public class OperationCodeModel
	{
		private readonly ApplicationCodeModel application;

		public Roperation Operation { get; private set; }
		public TypeCodeModel ReturnModel { get; private set; }
		public List<ParameterCodeModel> Parameters { get; private set; }
		public List<List<ParameterCodeModel>> Groups { get; private set; }

		public OperationCodeModel(ApplicationCodeModel application, Roperation operation)
		{
			this.application = application;
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
		public ApplicationCodeModel Application { get { return application; } }

		public string GetName(int mode)
		{
			return application.Configuration.GetName(this, mode);
		}

		public bool MarkedAs(string mark)
		{
			return Operation.MarkedAs(mark);
		}

		#region Equality & Hashcode

		protected bool Equals(OperationCodeModel other)
		{
			return Equals(Operation, other.Operation);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((OperationCodeModel)obj);
		}

		public override int GetHashCode()
		{
			return (Operation != null ? Operation.GetHashCode() : 0);
		}

		#endregion
	}
}
