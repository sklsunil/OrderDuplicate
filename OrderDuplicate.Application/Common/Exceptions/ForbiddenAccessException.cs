namespace OrderDuplicate.Application.Common.Exceptions;

public class ForbiddenAccessException(string message) : CustomException(message, null, System.Net.HttpStatusCode.Forbidden)
{
}
