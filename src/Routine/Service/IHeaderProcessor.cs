using System.Collections.Generic;

namespace Routine.Service;

public interface IHeaderProcessor
{
    void Process(IDictionary<string, string> responseHeaders);
}
