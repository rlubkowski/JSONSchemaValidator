using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SchemaValidator.Mapping;
using SchemaValidator.Model.Output;

namespace SchemaValidator
{
    public sealed class Validator
    {
        private readonly ErrorToValidationErrorMapper _errorToValidationErrorMapper;
        private readonly JObjectToDictionaryMapper _jObjectToDictionaryMapper;
        private readonly SchemaReader _schemaReader;
        private readonly ValidationService _validationService;
        public Validator()
        {
            _errorToValidationErrorMapper = new ErrorToValidationErrorMapper();
            _jObjectToDictionaryMapper = new JObjectToDictionaryMapper();
            _schemaReader = new SchemaReader();
            _validationService = new ValidationService();
        }

        public async Task<bool> ValidateAsync(string inputJsonPath, string schemaJsonPath, string outputJsonPath)
        {
            try
            {   
                var schemaResult = await _schemaReader.GetSchema(schemaJsonPath);

                if (schemaResult.IsError)
                {
                    var schemaErrorResult = new OutputResult
                    {
                        Record = new JObject(),
                        IsValid = false,
                        ErrorMessages = new List<ValidationError>
                            {
                                new ValidationError
                                {
                                    ErrorType = $"{schemaResult.FirstError.Code}",
                                    ErrorMessage = schemaResult.FirstError.Description,
                                    Field = "Schema"
                                }
                            }
                    };

                    await WriteOutputAsync(new List<OutputResult> { schemaErrorResult }, outputJsonPath);
                    return false;
                }

                var schema = schemaResult.Value;
                
                var validationResults = new List<OutputResult>();

                using (Stream stream = File.OpenRead(inputJsonPath))
                using (StreamReader streamReader = new StreamReader(stream))
                using (JsonTextReader reader = new JsonTextReader(streamReader))
                {
                    reader.SupportMultipleContent = true;

                    var serializer = new JsonSerializer();

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            JObject? record = serializer.Deserialize<JObject>(reader);
                            if (record != null)
                            {
                                var recordDict = _jObjectToDictionaryMapper.Map(record);
                                var validationResult = _validationService.Validate(recordDict, schema);
                                var result = new OutputResult
                                {
                                    Record = record,
                                    IsValid = !validationResult.IsError,
                                    ErrorMessages = validationResult.IsError
                                        ? [.. _errorToValidationErrorMapper.Map(validationResult.Errors)]
                                        : new List<ValidationError>()
                                };
                                validationResults.Add(result);
                            }
                        }
                    }
                }

                await WriteOutputAsync(validationResults, outputJsonPath);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task WriteOutputAsync(List<OutputResult> results, string outputPath)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(results, settings);
            await File.WriteAllTextAsync($"{outputPath}\\output.json", json);
        }
    }
}
