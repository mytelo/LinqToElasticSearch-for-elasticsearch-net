using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LinqToElasticSearch.JsonConverter
{
    public class JsonEnumConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override System.Text.Json.Serialization.JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            System.Text.Json.Serialization.JsonConverter converter = (System.Text.Json.Serialization.JsonConverter)Activator.CreateInstance(
                typeof(JsonEnumConverter<>).MakeGenericType(typeToConvert),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: null,
                culture: null);

            return converter;
        }
    }
    internal class JsonEnumConverter<T> : JsonConverter<T>
            where T : struct, Enum
    {
        public override bool HandleNull => true;

        public override bool CanConvert(Type type)
        {
            return type.IsEnum;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (T)Enum.ToObject(typeToConvert, reader.GetUInt64());
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(Convert.ToUInt64(value));
        }
    }
}
