using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Routine.Core
{
    public class ParameterModel
    {
        public List<string> Marks { get; set; } = new List<string>();
        public List<int> Groups { get; set; } = new List<int>();

        public string Name { get; set; }
        public string ViewModelId { get; set; }
        public bool IsList { get; set; }
        public bool IsOptional { get; set; }
        public VariableData DefaultValue { get; set; } = new VariableData();

        public ParameterModel() { }
        public ParameterModel(IDictionary<string, object> model)
        {
            if (model == null) return;

            if (model.TryGetValue("Marks", out var marks))
            {
                Marks = ((IEnumerable)marks).Cast<string>().ToList();
            }

            if (model.TryGetValue("Groups", out var groups))
            {
                Groups = ((IEnumerable)groups).Cast<int>().ToList();
            }

            if (model.TryGetValue("Name", out var name))
            {
                Name = (string)name;
            }

            if (model.TryGetValue("ViewModelId", out var viewModelId))
            {
                ViewModelId = (string)viewModelId;
            }

            if (model.TryGetValue("IsList", out var isList))
            {
                IsList = (bool)isList;
            }

            if (model.TryGetValue("IsOptional", out var isOptional))
            {
                IsOptional = (bool)isOptional;
            }

            if (model.TryGetValue("DefaultValue", out var defaultValue))
            {
                DefaultValue = new VariableData((IDictionary<string, object>)defaultValue);
            }
        }

        #region ToString & Equality

        public override string ToString()
        {
            return
                $"[ParameterModel: [Marks: {Marks.ToItemString()}, Groups: {Groups.ToItemString()}, " +
                $"Name: {Name}, ViewModelId: {ViewModelId}, IsList: {IsList}, IsOptional: {IsOptional}, " +
                $"DefaultValue: {DefaultValue}]]";
        }

        protected bool Equals(ParameterModel other)
        {
            return Marks.ItemEquals(other.Marks) && Groups.ItemEquals(other.Groups) && string.Equals(Name, other.Name) &&
                   string.Equals(ViewModelId, other.ViewModelId) && IsList == other.IsList && IsOptional == other.IsOptional &&
                   Equals(DefaultValue, other.DefaultValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((ParameterModel)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Marks != null ? Marks.GetItemHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Groups != null ? Groups.GetItemHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ViewModelId != null ? ViewModelId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsList.GetHashCode();
                hashCode = (hashCode * 397) ^ IsOptional.GetHashCode();
                hashCode = (hashCode * 397) ^ (DefaultValue != null ? DefaultValue.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion
    }
}
