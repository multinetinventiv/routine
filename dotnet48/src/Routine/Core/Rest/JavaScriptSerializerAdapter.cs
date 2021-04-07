using System.Web.Script.Serialization;

namespace Routine.Core.Rest
{
	public class JavaScriptSerializerAdapter : IJsonSerializer
	{
		private readonly JavaScriptSerializer javaScriptSerializer;

		public JavaScriptSerializerAdapter(JavaScriptSerializer javaScriptSerializer)
		{
			this.javaScriptSerializer = javaScriptSerializer;
		}

		public object DeserializeObject(string jsonString)
		{
			return javaScriptSerializer.DeserializeObject(jsonString);
		}

		public T Deserialize<T>(string jsonString)
		{
			return javaScriptSerializer.Deserialize<T>(jsonString);
		}

		public string Serialize(object @object)
		{
			return javaScriptSerializer.Serialize(@object);
		}
	}

	public static class ContextBuilderJavaScriptSerializerExtensions
	{
		public static ContextBuilder UsingJavaScriptSerializer(this ContextBuilder source, int maxJsonLength) { return source.UsingSerializer(new JavaScriptSerializerAdapter(new JavaScriptSerializer { MaxJsonLength = maxJsonLength })); }
		public static ContextBuilder UsingJavaScriptSerializer(this ContextBuilder source, int maxJsonLength, int recursionLimit) { return source.UsingSerializer(new JavaScriptSerializerAdapter(new JavaScriptSerializer { MaxJsonLength = maxJsonLength, RecursionLimit = recursionLimit })); }
	}
}