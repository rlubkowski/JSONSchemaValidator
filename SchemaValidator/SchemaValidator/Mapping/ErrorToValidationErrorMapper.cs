using ErrorOr;
using SchemaValidator.Model.Output;

namespace SchemaValidator.Mapping
{
    internal class ErrorToValidationErrorMapper
    {
        internal IEnumerable<ValidationError> Map(IEnumerable<Error> errors)
        {
            foreach (var error in errors)
            {
                string field = string.Empty;
                string errorMessage = error.Description;

                if (error.Description.Contains("Field '") && error.Description.Contains("'"))
                {
                    int startIndex = error.Description.IndexOf("Field '") + 7;
                    int endIndex = error.Description.IndexOf("'", startIndex);
                    field = error.Description.Substring(startIndex, endIndex - startIndex);
                }

               yield return new ValidationError
                {
                    ErrorType = error.Code,
                    ErrorMessage = errorMessage,
                    Field = field
                };
            }
        }
    }
}
