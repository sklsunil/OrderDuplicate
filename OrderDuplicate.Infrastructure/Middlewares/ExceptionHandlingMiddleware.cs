using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Exceptions;

using Microsoft.AspNetCore.Http;

using System.Net;


namespace OrderDuplicate.Infrastructure.Middlewares;

internal class ExceptionHandlingMiddleware : IMiddleware
{

    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            string userId = null;
            var responseModel = await Result.FailureAsync(new string[] { exception.Message });
            var response = context.Response;
            response.ContentType = "application/json";
            if (exception is not CustomException && exception.InnerException != null)
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
            }
            if (!string.IsNullOrEmpty(exception.Message))
            {
                responseModel.Errors = new string[] { exception.Message };
            }
            switch (exception)
            {
                case CustomException e:
                    response.StatusCode = (int)e.StatusCode;
                    if (e.ErrorMessages is not null)
                    {
                        responseModel.Errors = e.ErrorMessages.ToArray();
                    }
                    break;
                case KeyNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
            //_logger.LogError(exception, $"{exception}. Request failed with Status Code {response.StatusCode}");
            await response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(responseModel));
        }
    }
}
