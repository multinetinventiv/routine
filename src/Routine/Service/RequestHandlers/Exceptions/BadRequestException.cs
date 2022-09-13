namespace Routine.Service.RequestHandlers.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(Exception inner) : base(inner.Message, inner) { }
}
