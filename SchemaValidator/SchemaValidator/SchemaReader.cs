using ErrorOr;
using SchemaValidator.Model.Schema;
using System.Collections.Immutable;
using System.Text.Json;

namespace SchemaValidator
{
    internal sealed class SchemaReader
    {
        internal async Task<ErrorOr<ImmutableDictionary<string, Rule>>> GetSchema(string schemaPath)
        {
            try
            {
                var schemaJson = await File.ReadAllTextAsync(schemaPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var schemaDict = JsonSerializer.Deserialize<Dictionary<string, Rule>>(schemaJson, options);

                if (schemaDict == null)
                    return Error.Failure(
                        code: "Schema.Invalid",
                        description: "Schema file is invalid or empty.");

                return schemaDict.ToImmutableDictionary();
            }
            catch (System.Exception ex)
            {
                return Error.Failure(
                    code: "Schema.ReadFailed",
                    description: $"Failed to read schema file: {ex.Message}");
            }
        }
    }
}
  