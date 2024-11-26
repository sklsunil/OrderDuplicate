namespace OrderDuplicate.Application.Common.Exceptions;
public class ConflictException(string message) : CustomException(message, null, System.Net.HttpStatusCode.Conflict)
{
}
