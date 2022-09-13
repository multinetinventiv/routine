namespace Routine.Engine;

public class ReturnTypesDoNotMatchException : Exception
{
    public ReturnTypesDoNotMatchException(IReturnable returnable, IType expected, IType actual)
        : base(
            $"{returnable.ParentType.Name}.{returnable.Name}: Expected return type is {expected}, but given return type is {actual}")
    { }
}
