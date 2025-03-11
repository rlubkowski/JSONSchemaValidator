using ErrorOr;
using SchemaValidator.Model.Schema;
using SchemaValidator.Validation.Rule;
using System.Collections.Immutable;

namespace SchemaValidator
{
    internal sealed class ValidationService
    {
        private readonly IEnumerable<IValidationRule> validationRules;

        internal ValidationService()
        {
            validationRules = new List<IValidationRule>
            {
                new MandatoryValidationRule(),
                new LengthValidationRule()
            };
        }

        internal ErrorOr<Success> Validate(ImmutableDictionary<string, object?> input, ImmutableDictionary<string, Rule> schema)
        {
            var errors = new List<Error>();

            foreach (var (fieldName, fieldValue) in input)
            {
                if (!schema.TryGetValue(fieldName, out var rule))
                {
                    continue;
                }

                foreach (var validationRule in validationRules)
                {
                    var result = validationRule.Validate(fieldName, fieldValue, rule);
                    if (result.IsError)
                    {
                        errors.AddRange(result.Errors);
                    }
                }
            }

            foreach (var (fieldName, rule) in schema)
            {
                if (rule.Mandatory && !input.ContainsKey(fieldName))
                {
                    errors.Add(Error.Validation(
                        "Field.MandatoryError",
                        $"Field '{fieldName}' is mandatory but missing."
                    ));
                }
            }

            return errors.Count > 0 ? errors : Result.Success;
        }
    }
}
