using System.Text.Json.Serialization;

namespace SchemaValidator.Model.Schema
{
    internal record Rule
    {
        [JsonPropertyName("length")]
        public int? Length { get; init; }

        [JsonPropertyName("mandatory")]
        public bool Mandatory { get; init; }
    }
}
