using Newtonsoft.Json;

namespace Routine.Core.Rest
{
	public class JavaScriptSerializerAdapter : IJsonSerializer
    {
        public object DeserializeObject(string jsonString)
        {
            return JsonConvert.DeserializeObject(jsonString);
        }

        public T Deserialize<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public string Serialize(object @object)
        {
            return JsonConvert.SerializeObject(@object);
        }

    }

	public static class ContextBuilderJavaScriptSerializerExtensions
	{
		public static ContextBuilder UsingJavaScriptSerializer(this ContextBuilder source, int maxJsonLength) { return source.UsingSerializer(new JavaScriptSerializerAdapter(new JavaScriptSerializer { MaxJsonLength = maxJsonLength })); }
		public static ContextBuilder UsingJavaScriptSerializer(this ContextBuilder source, int maxJsonLength, int recursionLimit) { return source.UsingSerializer(new JavaScriptSerializerAdapter(new JavaScriptSerializer { MaxJsonLength = maxJsonLength, RecursionLimit = recursionLimit })); }
	}
}