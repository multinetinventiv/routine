using System.Collections.Generic;

namespace Routine.Core.Service
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

	public class ObjectModel
	{
		public string Id { get; set; }
		public List<string> Marks { get; set; }

		public string Name { get; set; }
		public string Module { get; set; }
		public bool IsValueModel { get; set; }
		public bool IsViewModel { get; set; }

		public List<string> ViewModelIds { get; set; }
		public List<MemberModel> Members { get; set; }
		public List<OperationModel> Operations { get; set; }

		public ObjectModel()
		{
			Marks = new List<string>();
			ViewModelIds = new List<string>();
			Members = new List<MemberModel>();
			Operations = new List<OperationModel>();
		}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ObjectModel: Id={0}, Marks={1}, Name={2}, Module={3}, IsValueModel={4}, IsViewModel={5}, ViewModelIds={6}, Members={7}, Operations={8}]", Id, Marks.ToItemString(), Name, Module, IsValueModel, IsViewModel, ViewModelIds.ToItemString(), Members.ToItemString(), Operations.ToItemString());
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(ObjectModel))
				return false;
			ObjectModel other = (ObjectModel)obj;
			return Id == other.Id && Marks.ItemEquals(other.Marks) && Name == other.Name && Module == other.Module && IsValueModel == other.IsValueModel && IsViewModel == other.IsViewModel && ViewModelIds.ItemEquals(other.ViewModelIds) && Members.ItemEquals(other.Members) && Operations.ItemEquals(other.Operations);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id != null ? Id.GetHashCode() : 0) ^ (Marks != null ? Marks.GetItemHashCode() : 0) ^ (Name != null ? Name.GetHashCode() : 0) ^ (Module != null ? Module.GetHashCode() : 0) ^ IsValueModel.GetHashCode() ^ IsViewModel.GetHashCode() ^ (ViewModelIds != null ? ViewModelIds.GetItemHashCode() : 0) ^ (Members != null ? Members.GetItemHashCode() : 0) ^ (Operations != null ? Operations.GetItemHashCode() : 0);
			}
		}

		#endregion
	}

	public class MemberModel
	{
		public string Id { get; set; }
		public List<string> Marks { get; set; }

		public string ViewModelId { get; set; }
		public bool IsList { get; set; }
		public bool IsHeavy { get; set; }

		public MemberModel() { Marks = new List<string>(); }

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[MemberModel: Id={0}, Marks={1}, ViewModelId={2}, IsList={3}, IsHeavy={4}]", Id, Marks.ToItemString(), ViewModelId, IsList, IsHeavy);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(MemberModel))
				return false;
			MemberModel other = (MemberModel)obj;
			return Id == other.Id && Marks.ItemEquals(other.Marks) && ViewModelId == other.ViewModelId && IsList == other.IsList && IsHeavy == other.IsHeavy;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id != null ? Id.GetHashCode() : 0) ^ (Marks != null ? Marks.GetItemHashCode() : 0) ^ (ViewModelId != null ? ViewModelId.GetHashCode() : 0) ^ IsList.GetHashCode() ^ IsHeavy.GetHashCode();
			}
		}

		#endregion
	}

	public class OperationModel
	{
		public string Id { get; set; }
		public List<string> Marks { get; set; }

		public bool IsHeavy { get; set; }

		public List<ParameterModel> Parameters { get; set; }
		public ResultModel Result { get; set; }

		public OperationModel()
		{
			Marks = new List<string>();
			Parameters = new List<ParameterModel>();
			Result = new ResultModel();
		}

		#region ToString & Equality
		public override string ToString()
		{
			return string.Format("[OperationModel: Id={0}, Marks={1}, IsHeavy={2}, Parameters={3}, Result={4}]", Id, Marks.ToItemString(), IsHeavy, Parameters.ToItemString(), Result);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(OperationModel))
				return false;
			OperationModel other = (OperationModel)obj;
			return Id == other.Id && Marks.ItemEquals(other.Marks) && IsHeavy == other.IsHeavy && Parameters.ItemEquals(other.Parameters) && object.Equals(Result, other.Result);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id != null ? Id.GetHashCode() : 0) ^ (Marks != null ? Marks.GetItemHashCode() : 0) ^ IsHeavy.GetHashCode() ^ (Parameters != null ? Parameters.GetItemHashCode() : 0) ^ (Result != null ? Result.GetHashCode() : 0);
			}
		}

		#endregion
	}

	public class ParameterModel
	{
		public string Id { get; set; }
		public List<string> Marks { get; set; }

		public string ViewModelId { get; set; }
		public bool IsList { get; set; }

		public ParameterModel() { Marks = new List<string>(); }

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ParameterModel: Id={0}, Marks={1}, ViewModelId={2}, IsList={3}]", Id, Marks.ToItemString(), ViewModelId, IsList);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(ParameterModel))
				return false;
			ParameterModel other = (ParameterModel)obj;
			return Id == other.Id && Marks.ItemEquals(other.Marks) && ViewModelId == other.ViewModelId && IsList == other.IsList;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id != null ? Id.GetHashCode() : 0) ^ (Marks != null ? Marks.GetItemHashCode() : 0) ^ (ViewModelId != null ? ViewModelId.GetHashCode() : 0) ^ IsList.GetHashCode();
			}
		}

		#endregion
	}	

	public class ResultModel
	{
		public string ViewModelId { get; set; }
		public bool IsList { get; set; }
		public bool IsVoid { get; set; }

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ResultModel: ViewModelId={0}, IsList={1}, IsVoid={2}]", ViewModelId, IsList, IsVoid);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(ResultModel))
				return false;
			ResultModel other = (ResultModel)obj;
			return ViewModelId == other.ViewModelId && IsList == other.IsList && IsVoid == other.IsVoid;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (ViewModelId != null ? ViewModelId.GetHashCode() : 0) ^ IsList.GetHashCode() ^ IsVoid.GetHashCode();
			}
		}

		#endregion
	}
}
