namespace Routine.Engine.Context;

public class TypeNotFoundException : Exception
{
    public string TypeId { get; }

    public TypeNotFoundException(string typeId)
        : base(
            $"Type could not be found with given type id: '{typeId}'. Make sure type id is correct and corresponding type is configured. " +
            "This can occur when a client with old version of service model tries to connect to server with a new version of service model. " +
            "Also make sure that ObjectService.GetApplicationModel is called before any other ObjectService methods are called " +
            "(This is because domain type of the expected type should be accessed via IType before trying to access via type id).")
    {
        TypeId = typeId;
    }
}
