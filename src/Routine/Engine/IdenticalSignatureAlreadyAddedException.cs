using System.Linq;
using System;

namespace Routine.Engine
{
    public class IdenticalSignatureAlreadyAddedException : Exception
    {
        public IdenticalSignatureAlreadyAddedException(IParametric parametric)
            : base(
                $"{parametric.Name}({string.Join(", ", parametric.Parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"))}) already added")
        { }
    }
}
