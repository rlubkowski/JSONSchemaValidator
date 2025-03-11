using ErrorOr;

namespace SchemaValidator.Validation.Rule
{
    internal interface IValidationRule
    {
        ErrorOr<Success> Validate(string fieldName, object fieldValue, Model.Schema.Rule field);
    }
}
