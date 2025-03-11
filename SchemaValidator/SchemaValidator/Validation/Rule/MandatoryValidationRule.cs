using ErrorOr;

namespace SchemaValidator.Validation.Rule
{
    internal class MandatoryValidationRule : IValidationRule
    {
        public ErrorOr<Success> Validate(string fieldName, object fieldValue, Model.Schema.Rule field)
        {
            if (field.Mandatory)
            {
                if (fieldValue == null)
                {
                    return ErrorOr.Error.Validation(
                        "Field.MandatoryError",
                        $"Field '{fieldName}' is mandatory but missing."
                    );
                }
                if (fieldValue is string stringValue && string.IsNullOrEmpty(stringValue))
                {
                    return ErrorOr.Error.Validation(
                        "Field.MandatoryError",
                        $"Field '{fieldName}' is mandatory but empty."
                    );
                }
            }
            return Result.Success;
        }
    }
}

