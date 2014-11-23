using System.Web.Script.Serialization;

namespace Routine.Core.Rest
{
	public interface IRestSerializer
	{
		T Deserialize<T>(string responseString);
		object Deserialize(string responseString);

		string Serialize(object @object);
	}
}
