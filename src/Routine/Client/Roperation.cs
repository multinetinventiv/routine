using System.Collections.Generic;
using Routine.Core;

namespace Routine.Client
{
	public class Roperation : Rparametric
	{
		private readonly OperationModel model;

		public Rtype ResultType { get; }

		public Roperation(OperationModel model, Rtype type)
			: base(model.Name, model.GroupCount, model.Parameters, model.Marks, type)
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

		public string Name => model.Name;
        public bool ResultIsVoid => model.Result.IsVoid;
        public bool ResultIsList => model.Result.IsList;

        public Rvariable Perform(Robject target, List<Rvariable> parameterVariables)
		{
			var parameterValues = new Dictionary<string, ParameterValueData>();
			foreach (var parameterVariable in parameterVariables)
			{
				var rparam = Parameter[parameterVariable.Name];
				var parameterValue = rparam.CreateParameterValueData(parameterVariable.List);
				parameterValues.Add(rparam.Name, parameterValue);
			}

			var resultData = Application.Service.Do(target.ReferenceData, model.Name, parameterValues);

			if (ResultIsVoid)
			{
				return new Rvariable(true);
			}

			return new Rvariable(Application, resultData, ResultType.Id);
		}

		#region Equality & Hashcode

		protected bool Equals(Roperation other)
		{
			return Equals(Type, other.Type) && Equals(model, other.model);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((Roperation)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (model != null ? model.GetHashCode() : 0);
			}
		}

		#endregion
	}
}
