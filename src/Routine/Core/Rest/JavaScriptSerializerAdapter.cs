
using System.Text.Json;

namespace Routine.Core.Rest
{
    public class JsonSerializerAdapter : IJsonSerializer
    {
        // private const int DEFAULT_RECURSION_LIMIT = 100;
        // private const int DEFAULT_MAX_JSON_LENGTH = 1 * 1024 * 1024;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public JsonSerializerAdapter(JsonSerializerOptions jsonSerializerOptions = null)
        {
            //todo: null ise default degerlerin muadilleri set edilmeli
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public object DeserializeObject(string jsonString)
        {
            return JsonSerializer.Deserialize<object>(jsonString, jsonSerializerOptions);
        }

        public T Deserialize<T>(string jsonString)
        {
            return JsonSerializer.Deserialize<T>(jsonString, jsonSerializerOptions);
        }

        public string Serialize(object @object)
        {
            return JsonSerializer.Serialize(@object, jsonSerializerOptions);
        }
    }

    public static class ContextBuilderJsonSerializerExtensions
    {
        public static ContextBuilder UsingJsonSerializer(this ContextBuilder source, int? maxJsonLength = null, int? recursionLimit = null) { return source.UsingSerializer(new JsonSerializerAdapter()); }
    }
}