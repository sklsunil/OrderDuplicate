namespace OrderDuplicate.Application.Common.Exceptions;
public class UnauthorizedException(string message) : CustomException(message, null, System.Net.HttpStatusCode.Unauthorized)
{
}

