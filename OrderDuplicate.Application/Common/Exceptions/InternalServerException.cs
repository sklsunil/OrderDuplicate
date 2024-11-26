namespace OrderDuplicate.Application.Common.Exceptions;
public class InternalServerException(string message, List<string>? errors = default) : CustomException(message, errors, System.Net.HttpStatusCode.InternalServerError)
{
}