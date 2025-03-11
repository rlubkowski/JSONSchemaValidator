using Newtonsoft.Json.Linq;

namespace SchemaValidator.Model.Output
{
    internal record OutputResult
    {
        public required JObject Record { get; set; }
        public bool IsValid { get; set; }
        public List<ValidationError> ErrorMessages { get; set; } = new List<ValidationError>();
    }
}
