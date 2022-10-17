namespace Routine.Engine;

public class OperationDoesNotExistException : Exception
{
    public OperationDoesNotExistException(string objectModelId, string operationName)
        : base($"Operation '{operationName}' does not exist on Object '{objectModelId}'") { }
}
