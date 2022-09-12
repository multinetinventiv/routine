using System.Threading.Tasks;

namespace Routine.Core.Rest;

public interface IRestClient
{
    RestResponse Get(string url, RestRequest request);
    RestResponse Post(string url, RestRequest request);

    Task<RestResponse> GetAsync(string url, RestRequest request);
    Task<RestResponse> PostAsync(string url, RestRequest request);
}
