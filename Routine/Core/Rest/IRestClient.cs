namespace Routine.Core.Rest
{
	public interface IRestClient
	{
		RestResponse Get(string url, params RestParameter[] parameters);
		RestResponse Post(string url, params RestParameter[] parameters);
	}
}

