using System.Collections.Generic;

namespace Routine.Core;

public class ResultModel
{
    public string ViewModelId { get; set; }
    public bool IsList { get; set; }
    public bool IsVoid { get; set; }

    public ResultModel() { }
    public ResultModel(IDictionary<string, object> model)
    {
        if (model == null) return;

        if (model.TryGetValue("ViewModelId", out var viewModelId))
        {
            ViewModelId = (string)viewModelId;
        }

        if (model.TryGetValue("IsList", out var isList))
        {
            IsList = (bool)isList;
        }

        if (model.TryGetValue("IsVoid", out var isVoid))
        {
            IsVoid = (bool)isVoid;
        }
    }

    #region ToString & Equality

    public override string ToString()
    {
        return $"[ResultModel: [ViewModelId: {ViewModelId}, IsList: {IsList}, IsVoid: {IsVoid}]]";
    }

    protected bool Equals(ResultModel other)
    {
        return string.Equals(ViewModelId, other.ViewModelId) && IsList == other.IsList && IsVoid == other.IsVoid;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((ResultModel)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (ViewModelId != null ? ViewModelId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ IsList.GetHashCode();
            hashCode = (hashCode * 397) ^ IsVoid.GetHashCode();
            return hashCode;
        }
    }

    #endregion
}
