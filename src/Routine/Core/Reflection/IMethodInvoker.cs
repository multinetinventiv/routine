using System.Threading.Tasks;

namespace Routine.Core.Reflection
{
    public interface IMethodInvoker
    {
        object Invoke(object target, params object[] args);
        Task<object> InvokeAsync(object target, params object[] args);
    }
}
