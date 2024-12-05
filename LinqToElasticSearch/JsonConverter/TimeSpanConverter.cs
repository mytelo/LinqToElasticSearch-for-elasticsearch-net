using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LinqToElasticSearch.JsonConverter
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override bool HandleNull => true;

        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new TimeSpan(reader.GetInt64());
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.Ticks);
        }
    }
}