using FluentValidation.Results;

namespace OrderDuplicate.Application.Common.Exceptions;

public class ValidationException(IEnumerable<ValidationFailure> failures) : CustomException(string.Empty, failures
             .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
             .Select(failureGroup => $"{string.Join(", ", failureGroup.Distinct().ToArray())}")
             .ToList(), System.Net.HttpStatusCode.UnprocessableEntity)
{
}
