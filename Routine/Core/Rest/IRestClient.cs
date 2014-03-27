namespace Routine.Core.Rest
{
	public interface IRestClient
	{
		string Get(string url, params RestParameter[] parameters);
		string Post(string url, params RestParameter[] parameters);
	}
}

