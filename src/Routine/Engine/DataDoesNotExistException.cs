namespace Routine.Engine;

public class DataDoesNotExistException : Exception
{
    public DataDoesNotExistException(string objectModelId, string dataName)
        : base($"Data '{dataName}' does not exist on Object '{objectModelId}'") { }
}
