using System;

namespace Routine.Engine;

public class ParameterTypesDoNotMatchException : Exception
{
    public ParameterTypesDoNotMatchException(IParameter parameter, IType expected, IType actual)
        : base(
            $"{parameter.Owner.ParentType.Name}.{parameter.Owner.Name}(...,{parameter.Name},...): Parameter's expected type is {expected}, but given parameter has a type of {actual}")
    { }
}
