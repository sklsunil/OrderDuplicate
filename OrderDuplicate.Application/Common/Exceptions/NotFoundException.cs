
namespace OrderDuplicate.Application.Common.Exceptions;

public class NotFoundException : CustomException
{


    public NotFoundException(string message)
        : base(message, null, System.Net.HttpStatusCode.NotFound)
    {
    }



    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.", null, System.Net.HttpStatusCode.NotFound)
    {
    }
}
