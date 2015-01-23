using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Client
{
	public class Rparameter
	{
		private readonly ParameterModel model;

		public Rparametric Owner { get; private set; }
		public Rtype ParameterType { get; private set; }

		public Rparameter(ParameterModel model, Rparametric owner)
		{
			this.model = model;

			Owner = owner;

			ParameterType = Application[model.ViewModelId];
		}

		public Rapplication Application { get { return Owner.Type.Application; } }
		public Rtype Type { get { return Owner.Type; } }
		public string Id { get { return model.Id; } }
		public bool IsList { get { return model.IsList; } }
		internal List<int> Groups { get { return model.Groups; } }
		public List<string> Marks { get { return model.Marks; } }

		public bool MarkedAs(string mark)
		{
			return model.Marks.Any(m => m == mark);
		}

		public Rvariable CreateVariable(params Robject[] robjs) { return CreateVariable(robjs.ToList()); }
		public Rvariable CreateVariable(List<Robject> robjs)
		{
			var result = new Rvariable(Id, robjs);

			if (!IsList)
			{
				return result.ToSingle();
			}

			return result;
		}

		internal ParameterValueData CreateParameterValueData(params Robject[] robjs) { return CreateParameterValueData(robjs.ToList()); }
		internal ParameterValueData CreateParameterValueData(List<Robject> robjs)
		{
			return new ParameterValueData
			{
				IsList = IsList,
				Values = robjs.Select(robj => robj.GetParameterData()).ToList()
			};
		}

		#region Equality & Hashcode

		protected bool Equals(Rparameter other)
		{
			return Equals(Owner, other.Owner) && Equals(model, other.model);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((Rparameter) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Owner != null ? Owner.GetHashCode() : 0)*397) ^ (model != null ? model.GetHashCode() : 0);
			}
		}

		#endregion
	}
}
