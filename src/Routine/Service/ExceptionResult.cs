using Routine.Core.Rest;
using System.Text.Json.Serialization;

namespace Routine.Service;

public class ExceptionResult
{
    [JsonConverter(typeof(BooleanJsonConverter))]
    public bool IsException => true;

    [JsonConverter(typeof(BooleanJsonConverter))]
    public bool IsHandled { get; }
    public string Type { get; }
    public string Message { get; }

    internal ExceptionResult(ExceptionResultData data) : this(data.Type, data.Message, data.IsHandled) { }
    public ExceptionResult() : this("NaN", "NaN", false) { }
    public ExceptionResult(string type, string message) : this(type, message, true) { }
    public ExceptionResult(string type, string message, bool isHandled)
    {
        Type = type;
        Message = message;
        IsHandled = isHandled;
    }
}

internal class ExceptionResultData
{
    public bool IsException { get; set; }
    public bool IsHandled { get; set; }
    public string Type { get; set; }
    public string Message { get; set; }
}
