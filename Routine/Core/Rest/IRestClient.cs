namespace Routine.Core.Rest
{
	public interface IRestClient
	{
		string Get(string url, params RestParameter[] parameters);
	}
}

