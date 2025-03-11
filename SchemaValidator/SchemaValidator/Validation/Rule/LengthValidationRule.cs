using ErrorOr;

namespace SchemaValidator.Validation.Rule
{
    internal class LengthValidationRule : IValidationRule
    {
        public ErrorOr<Success> Validate(string fieldName, object fieldValue, Model.Schema.Rule field)
        {
            if (!field.Length.HasValue || fieldValue == null || string.IsNullOrEmpty(fieldValue.ToString()))
                return Result.Success;

            var stringValue = fieldValue.ToString() ?? string.Empty;
            return stringValue.Length > field.Length.Value
                ?    ErrorOr.Error.Validation(
                        "Field.LengthExceeded",
                        $"Field '{fieldName}' has exceeded it's maximum length of {field.Length.Value}."
                    )
                : Result.Success;
        }
    }
}
