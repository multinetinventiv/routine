using System.Collections.Generic;

namespace Routine.Core
{
	public class ObjectModel
	{
		public string Id { get; set; }
		public List<string> Marks { get; set; }

		public string Name { get; set; }
		public string Module { get; set; }
		public bool IsValueModel { get; set; }
		public bool IsViewModel { get; set; }

		public List<string> ViewModelIds { get; set; }
		public List<string> ActualModelIds { get; set; }
		public InitializerModel Initializer { get; set; }
		public List<MemberModel> Members { get; set; }
		public List<OperationModel> Operations { get; set; }
		public List<ObjectData> StaticInstances { get; set; } 

		public ObjectModel()
		{
			Marks = new List<string>();
			ViewModelIds = new List<string>();
			ActualModelIds = new List<string>();
			Initializer = new InitializerModel();
			Members = new List<MemberModel>();
			Operations = new List<OperationModel>();
			StaticInstances = new List<ObjectData>();
		}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ObjectModel: Id={0}, Marks={1}, Name={2}, Module={3}, IsValueModel={4}, IsViewModel={5}, ViewModelIds={6}, ActualModelIds={7}, Initializer={8}, Members={9}, Operations={10}, StaticInstances={11}]",
				Id, Marks.ToItemString(), Name, Module, IsValueModel, IsViewModel, ViewModelIds.ToItemString(), ActualModelIds.ToItemString(), Initializer, Members.ToItemString(), Operations.ToItemString(), StaticInstances.ToItemString());
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(ObjectModel))
				return false;

			var other = (ObjectModel)obj;

			return 
				Id == other.Id && Marks.ItemEquals(other.Marks) && Name == other.Name && Module == other.Module && IsValueModel == other.IsValueModel && IsViewModel == other.IsViewModel &&
				ViewModelIds.ItemEquals(other.ViewModelIds) && ActualModelIds.ItemEquals(other.ActualModelIds) && Initializer.Equals(other.Initializer) && Members.ItemEquals(other.Members) && Operations.ItemEquals(other.Operations) && StaticInstances.ItemEquals(other.StaticInstances);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id != null ? Id.GetHashCode() : 0) ^ (Marks != null ? Marks.GetItemHashCode() : 0) ^ (Name != null ? Name.GetHashCode() : 0) ^ (Module != null ? Module.GetHashCode() : 0) ^ IsValueModel.GetHashCode() ^ IsViewModel.GetHashCode() ^ (ViewModelIds != null ? ViewModelIds.GetItemHashCode() : 0) ^ (ActualModelIds != null ? ActualModelIds.GetItemHashCode() : 0) ^ (Initializer != null ? Initializer.GetHashCode() : 0) ^ (Members != null ? Members.GetItemHashCode() : 0) ^ (Operations != null ? Operations.GetItemHashCode() : 0) ^ (StaticInstances != null ? StaticInstances.GetItemHashCode() : 0);
			}
		}

		#endregion
	}
}