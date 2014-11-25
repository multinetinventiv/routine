using Routine.Core;

namespace Routine.Client
{
	public class Rinitializer : Rparametric
	{
		private readonly InitializerModel model;

		public Rinitializer(InitializerModel model, Rtype type)
			: base(Constants.INITIALIZER_ID, model.GroupCount, model.Parameters, model.Marks, type)
		{
			this.model = model;
		}

		#region Equality & Hashcode

		protected bool Equals(Rinitializer other)
		{
			return Equals(model, other.model);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Rinitializer)obj);
		}

		public override int GetHashCode()
		{
			return (model != null ? model.GetHashCode() : 0);
		} 

		#endregion

	}
}
