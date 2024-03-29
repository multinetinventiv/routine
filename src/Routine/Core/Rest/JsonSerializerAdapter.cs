using System.Text.Json.Serialization;
using System.Text.Json;

namespace Routine.Core.Rest;

public class JsonSerializerAdapter : IJsonSerializer
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public JsonSerializerAdapter(JsonSerializerOptions jsonSerializerOptions = null)
    {
        if (jsonSerializerOptions != null)
        {
            jsonSerializerOptions.Converters.Add(new DictionaryStringObjectJsonConverter());
            jsonSerializerOptions.Converters.Add(new BooleanJsonConverter());
            jsonSerializerOptions.Converters.Add(new ObjectConverter());
        }

        _jsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions { Converters = { new DictionaryStringObjectJsonConverter(), new BooleanJsonConverter(), new ObjectConverter() } };
    }

    public object DeserializeObject(string jsonString) => !string.IsNullOrWhiteSpace(jsonString) ? JsonSerializer.Deserialize<object>(jsonString, _jsonSerializerOptions) : null;
    public T Deserialize<T>(string jsonString) => !string.IsNullOrWhiteSpace(jsonString) ? JsonSerializer.Deserialize<T>(jsonString, _jsonSerializerOptions) : default;
    public string Serialize(object @object) => JsonSerializer.Serialize(@object, _jsonSerializerOptions);
}

internal class ObjectConverter : JsonConverter<object>
{
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True:
                return true;
            case JsonTokenType.False:
                return false;
            case JsonTokenType.Number when reader.TryGetInt32(out var @int):
                return @int;
            case JsonTokenType.Number when reader.TryGetInt64(out var @long):
                return @long;
            case JsonTokenType.Number:
                return reader.GetDouble();
            case JsonTokenType.String when reader.TryGetDateTime(out var datetime):
                return datetime;
            case JsonTokenType.String:
                return reader.GetString();
            case JsonTokenType.StartObject:
                {
                    var dictionary = new Dictionary<string, object>();
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                        {
                            return dictionary;
                        }

                        if (reader.TokenType != JsonTokenType.PropertyName)
                        {
                            throw new JsonException("JsonTokenType was not PropertyName");
                        }

                        var propertyName = reader.GetString();

                        if (string.IsNullOrWhiteSpace(propertyName))
                        {
                            throw new JsonException("Failed to get property name");
                        }

                        reader.Read();

                        dictionary.Add(propertyName, ExtractValue(ref reader, options));
                    }

                    return dictionary;
                }
            case JsonTokenType.StartArray:
                {
                    var list = new List<object>();
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            break;
                        }

                        list.Add(ExtractValue(ref reader, options));
                    }

                    return list.ToArray();
                }
            default:
                throw new NotSupportedException("ObjectConverter cannot convert this json to object");
        }
    }

    private object ExtractValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                if (reader.TryGetDateTime(out var date))
                {
                    return date;
                }
                return reader.GetString();
            case JsonTokenType.False:
                return false;
            case JsonTokenType.True:
                return true;
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.Number when reader.TryGetInt32(out var @int):
                return @int;
            case JsonTokenType.Number when reader.TryGetInt64(out var @long):
                return @long;
            case JsonTokenType.Number:
                return reader.GetDouble();
            case JsonTokenType.StartObject:
                return Read(ref reader, null, options);
            case JsonTokenType.StartArray:
                var list = new List<object>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    list.Add(ExtractValue(ref reader, options));
                }
                return list.ToArray();
            default:
                throw new JsonException($"'{reader.TokenType}' is not supported");
        }
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options) => JsonSerializer.Serialize(writer, value, value.GetType(), options);
}

internal class DictionaryStringObjectJsonConverter : JsonConverter<Dictionary<string, object>>
{
    public override Dictionary<string, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
        }

        var dictionary = new Dictionary<string, object>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dictionary;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("JsonTokenType was not PropertyName");
            }

            var propertyName = reader.GetString();

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new JsonException("Failed to get property name");
            }

            reader.Read();

            dictionary.Add(propertyName, ExtractValue(ref reader, options));
        }

        return dictionary;
    }

    private object ExtractValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return reader.TryGetDateTime(out var date) ? date : reader.GetString();
            case JsonTokenType.False:
                return false;
            case JsonTokenType.True:
                return true;
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.Number:
                return reader.TryGetInt32(out var result) ? result : reader.GetDecimal();
            case JsonTokenType.StartObject:
                return Read(ref reader, null, options);
            case JsonTokenType.StartArray:
                var list = new List<object>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    list.Add(ExtractValue(ref reader, options));
                }
                return list.ToArray();
            default:
                throw new JsonException($"'{reader.TokenType}' is not supported");
        }
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var key in value.Keys)
        {
            HandleValue(writer, key, value[key]);
        }

        writer.WriteEndObject();
    }

    private static void HandleValue(Utf8JsonWriter writer, string key, object objectValue)
    {
        if (key != null)
        {
            writer.WritePropertyName(key);
        }

        switch (objectValue)
        {
            case string stringValue:
                writer.WriteStringValue(stringValue);
                break;
            case DateTime dateTime:
                writer.WriteStringValue(dateTime);
                break;
            case long longValue:
                writer.WriteNumberValue(longValue);
                break;
            case int intValue:
                writer.WriteNumberValue(intValue);
                break;
            case float floatValue:
                writer.WriteNumberValue(floatValue);
                break;
            case double doubleValue:
                writer.WriteNumberValue(doubleValue);
                break;
            case decimal decimalValue:
                writer.WriteNumberValue(decimalValue);
                break;
            case bool boolValue:
                writer.WriteBooleanValue(boolValue);
                break;
            case Dictionary<string, object> dict:
                writer.WriteStartObject();
                foreach (var item in dict)
                {
                    HandleValue(writer, item.Key, item.Value);
                }
                writer.WriteEndObject();
                break;
            case object[] array:
                writer.WriteStartArray();
                foreach (var item in array)
                {
                    HandleValue(writer, item);
                }
                writer.WriteEndArray();
                break;
            case List<object> list:
                writer.WriteStartArray();
                foreach (var item in list)
                {
                    HandleValue(writer, item);
                }
                writer.WriteEndArray();
                break;
            default:
                writer.WriteNullValue();
                break;
        }
    }

    private static void HandleValue(Utf8JsonWriter writer, object value) => HandleValue(writer, null, value);
}

public class BooleanJsonConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            _ => bool.Parse(reader.GetString())
        };

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) => writer.WriteBooleanValue(value);
}
