using System.Collections.Generic;
using Routine.Core;

namespace Routine.Client
{
	public class Roperation : Rparametric
	{
		private readonly OperationModel model;

		public Rtype ResultType { get; private set; }

		public Roperation(OperationModel model, Rtype type)
			: base(model.GroupCount, model.Parameters, model.Marks, type)
		{
			this.model = model;

			if (model.Result.IsVoid)
			{
				ResultType = Rtype.Void;
			}
			else
			{
				ResultType = Application[model.Result.ViewModelId];
			}
		}

		public string Id { get { return model.Id; } }
		public bool ResultIsVoid { get { return model.Result.IsVoid; } }
		public bool ResultIsList { get { return model.Result.IsList; } }

		public Rvariable Perform(Robject target, List<Rvariable> parameterVariables)
		{
			var parameterValues = new Dictionary<string, ParameterValueData>();
			foreach (var parameterVariable in parameterVariables)
			{
				var rparam = Parameter[parameterVariable.Name];
				var parameterValue = rparam.CreateParameterValueData(parameterVariable.List);
				parameterValues.Add(rparam.Id, parameterValue);
			}

			var resultData = Application.Service.PerformOperation(target.ObjectReferenceData, model.Id, parameterValues);

			if (ResultIsVoid)
			{
				return new Rvariable(true);
			}

			return new Rvariable(Application, resultData);
		}

		#region Equality & Hashcode

		protected bool Equals(Roperation other)
		{
			return Equals(model, other.model);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) { return false; }
			if (ReferenceEquals(this, obj)) { return true; }
			if (obj.GetType() != GetType()) { return false; }

			return Equals((Roperation)obj);
		}

		public override int GetHashCode()
		{
			return (model != null ? model.GetHashCode() : 0);
		}

		#endregion
	}
}
