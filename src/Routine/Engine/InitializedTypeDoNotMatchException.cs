using System;

namespace Routine.Engine
{
    public class InitializedTypeDoNotMatchException : Exception
    {
        public InitializedTypeDoNotMatchException(IConstructor constructor, IType expected, IType actual)
            : base(
                $"{constructor.ParentType.Name}.{constructor.Name}: Expected initialized type is {expected}, but given initialized type is {actual}")
        { }
    }
}
