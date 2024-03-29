namespace Routine.Core.Rest;

public class RestResponse
{
    public static readonly RestResponse Empty = new(string.Empty);

    public string Body { get; }
    public Dictionary<string, string> Headers { get; }

    public RestResponse(string body) : this(body, new Dictionary<string, string>()) { }
    public RestResponse(string body, IDictionary<string, string> headers)
    {
        Body = body;
        Headers = new(headers);
    }

    #region ToString & Equality

    public override string ToString()
    {
        return $"[RestResponse: Body={Body}, Headers={Headers.ToKeyValueString()}]";
    }

    protected bool Equals(RestResponse other)
    {
        return string.Equals(Body, other.Body) && Headers.KeyValueEquals(other.Headers);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((RestResponse)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((Body != null ? Body.GetHashCode() : 0) * 397) ^ (Headers != null ? Headers.GetKeyValueHashCode() : 0);
        }
    }

    #endregion
}
