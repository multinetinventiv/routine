namespace Routine.Engine;

public class MissingParameterException : Exception
{
    public MissingParameterException(string objectModelId, string operationName, string parameterName)
        : base($"Parameter '{parameterName}' was not given for Operation '{operationName} on Object '{objectModelId}'") { }
}
