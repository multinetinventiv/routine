using System.Collections.Generic;

namespace Routine.Core
{
	public class ApplicationModel
	{
		public List<ObjectModel> Models { get; set; }

		public ApplicationModel() { Models = new List<ObjectModel>(); }

		#region ToString & Equality
		public override string ToString()
		{
			return string.Format("[ApplicationModel: Models={0}]", Models.ToItemString());
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(ApplicationModel))
				return false;
			ApplicationModel other = (ApplicationModel)obj;
			return Models.ItemEquals(other.Models);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Models != null ? Models.GetItemHashCode() : 0);
			}
		}

		#endregion
	}
}