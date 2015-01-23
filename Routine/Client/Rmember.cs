using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Client
{
	public class Rmember
	{
		private readonly MemberModel model;

		public Rtype Type { get; private set; }
		public Rtype MemberType { get; private set; }

		public Rmember(MemberModel model, Rtype type)
		{
			this.model = model;

			Type = type;
			MemberType = Application[model.ViewModelId];
		}

		public Rapplication Application { get { return Type.Application; } }
		public string Id { get { return model.Id; } }
		public bool IsList { get { return model.IsList; } }
		public List<string> Marks { get { return model.Marks; } }

		public bool MarkedAs(string mark)
		{
			return model.Marks.Any(m => m == mark);
		}

		#region Equality & Hashcode

		protected bool Equals(Rmember other)
		{
			return Equals(Type, other.Type) && Equals(model, other.model);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Rmember) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Type != null ? Type.GetHashCode() : 0)*397) ^ (model != null ? model.GetHashCode() : 0);
			}
		}

		#endregion
	}
}
