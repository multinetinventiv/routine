namespace Routine.Core;

public class ParameterData
{
    public string? ModelId { get; set; }
    public string? Id { get; set; }
    public Dictionary<string, ParameterValueData> InitializationParameters { get; set; } = new();

    #region ToString & Equality

    public override string ToString()
    {
        return
            $"[ParameterData: [ModelId: {ModelId}, Id: {Id}, InitializationParameters: {InitializationParameters.ToKeyValueString()}]]";
    }

    protected bool Equals(ParameterData other)
    {
        return string.Equals(ModelId, other.ModelId) && string.Equals(Id, other.Id) && InitializationParameters.KeyValueEquals(other.InitializationParameters);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((ParameterData)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (ModelId != null ? ModelId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Id != null ? Id.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (InitializationParameters != null ? InitializationParameters.GetKeyValueHashCode() : 0);
            return hashCode;
        }
    }

    #endregion
}
