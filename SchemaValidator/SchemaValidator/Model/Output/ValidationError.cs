using System.Text.Json.Serialization;

namespace SchemaValidator.Model.Output
{
    internal record ValidationError
    {
        [JsonPropertyName("errorType")]
        public required string ErrorType { get; set; }

        [JsonPropertyName("errorMessage")]
        public required string ErrorMessage { get; set; }

        [JsonPropertyName("field")]
        public required string Field { get; set; }
    }
}
