namespace Routine.Core;

public class OperationWithObjectModel
{
    public ObjectModel ObjectModel { get; set; }
    public OperationModel OperationModel { get; set; }

    public OperationWithObjectModel() : this(new ObjectModel(), new OperationModel()) { }
    public OperationWithObjectModel(ObjectModel objectModel, OperationModel operationModel)
    {
        ObjectModel = objectModel;
        OperationModel = operationModel;
    }

    #region ToString & Equality

    public override string ToString()
    {
        return $"[OperationWithObjectModel:[ObjectModel: {ObjectModel}, OperationModel: {OperationModel}]]";
    }

    protected bool Equals(OperationWithObjectModel other)
    {
        return Equals(ObjectModel, other.ObjectModel) && Equals(OperationModel, other.OperationModel);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((OperationWithObjectModel)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return
                ((ObjectModel != null ? ObjectModel.GetHashCode() : 0) * 397) ^
                (OperationModel != null ? OperationModel.GetHashCode() : 0);
        }
    }

    #endregion
}
