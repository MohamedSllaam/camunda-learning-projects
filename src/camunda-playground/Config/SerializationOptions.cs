namespace camunda_playground.Config;

using System.Text.Json;
using System.Text.Json.Serialization;

public static class SerializationOptions
{
    public static readonly JsonSerializerOptions IgnoreNull =
        new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
}
