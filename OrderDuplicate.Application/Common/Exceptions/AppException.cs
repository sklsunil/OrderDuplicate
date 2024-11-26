using System.Net;

namespace OrderDuplicate.Application.Common.Exceptions
{
    public class AppException : ApplicationException
    {
        public List<string> Exceptions { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public AppException(List<string> exceptions, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            this.Exceptions = exceptions;
            this.StatusCode = statusCode;
        }
    }
}
