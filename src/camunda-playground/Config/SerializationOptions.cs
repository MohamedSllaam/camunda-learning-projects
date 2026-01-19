using System.Text.Json.Serialization.Metadata;

namespace camunda_playground.Config
{
    public static class SerializationOptions
    {
        public static JsonTypeInfo IgnoreNullSerializationOption { get; set; }
    }
}
