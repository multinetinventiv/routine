namespace Routine.Core.Rest
{
	public interface IRestClient
	{
		RestResponse Get(string url, RestRequest request);
		RestResponse Post(string url, RestRequest request);
	}
}

