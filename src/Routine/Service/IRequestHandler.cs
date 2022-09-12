using System.Threading.Tasks;

namespace Routine.Service;

public interface IRequestHandler
{
    Task WriteResponse();
}
