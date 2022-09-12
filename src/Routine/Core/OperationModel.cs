using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Routine.Core;

public class OperationModel
{
    public List<string> Marks { get; set; } = new List<string>();
    public int GroupCount { get; set; }

    public string Name { get; set; }
    internal Dictionary<string, ParameterModel> Parameter { get; private set; } = new Dictionary<string, ParameterModel>();
    public ResultModel Result { get; set; } = new ResultModel();

    public OperationModel() { }
    public OperationModel(IDictionary<string, object> model)
    {
        if (model == null) return;

        if (model.TryGetValue("Marks", out var marks))
        {
            Marks = ((IEnumerable)marks).Cast<string>().ToList();
        }

        if (model.TryGetValue("GroupCount", out var groupCount))
        {
            GroupCount = (int)groupCount;
        }

        if (model.TryGetValue("Name", out var name))
        {
            Name = (string)name;
        }

        if (model.TryGetValue("Parameters", out var parameters))
        {
            Parameters = ((IEnumerable)parameters).Cast<IDictionary<string, object>>().Select(p => new ParameterModel(p)).ToList();
        }

        if (model.TryGetValue("Result", out var result))
        {
            Result = new ResultModel((IDictionary<string, object>)result);
        }
    }

    public List<ParameterModel> Parameters
    {
        get => Parameter.Values.ToList();
        set => Parameter = value.ToDictionary(kvp => kvp.Name, kvp => kvp);
    }

    public ParameterModel GetParameter(string name)
    {
        Parameter.TryGetValue(name, out var result);

        return result;
    }

    public void AddParameter(string name, ParameterModel model) => Parameter.Add(name, model);

    #region ToString & Equality

    public override string ToString()
    {
        return
            $"[OperationModel: [Marks: {Marks.ToItemString()}, GroupCount: {GroupCount}, Name: {Name}, Parameters: {Parameters.ToItemString()}, Result: {Result}]]";
    }

    protected bool Equals(OperationModel other)
    {
        return Marks.ItemEquals(other.Marks) && GroupCount == other.GroupCount && string.Equals(Name, other.Name) && Parameters.ItemEquals(other.Parameters) && Equals(Result, other.Result);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((OperationModel)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (Marks != null ? Marks.GetItemHashCode() : 0);
            hashCode = (hashCode * 397) ^ GroupCount;
            hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Parameters != null ? Parameters.GetItemHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Result != null ? Result.GetHashCode() : 0);
            return hashCode;
        }
    }

    #endregion
}
