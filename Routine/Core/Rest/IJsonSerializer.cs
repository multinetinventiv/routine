namespace Routine.Core.Rest
{
	public interface IJsonSerializer
	{
		object DeserializeObject(string jsonString);
		T Deserialize<T>(string jsonString);
		string Serialize(object @object);
	}
}