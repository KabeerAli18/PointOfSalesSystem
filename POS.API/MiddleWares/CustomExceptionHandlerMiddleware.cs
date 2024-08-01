using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;


/// <summary>
/// This is the Custom Middle ware to handle the exceptions from all the Layers of web api project, handling 500,501, 200,401,403 etc
/// </summary>
namespace POS.API.MiddleWares
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // Handle cases where a 403 Forbidden response is set
                if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    await HandleForbiddenAsync(context);
                }
                // Handle cases where a 401 Unauthorized response is set
                else if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    await HandleUnauthorizedAsync(context);
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred, I am Custom Middleware.";
            var exceptionType = exception.GetType().Name;

            if (exception is ArgumentException)
            {
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized;
                message = "Unauthorized access.";
            }
            else if (exception is CosmosException cosmosException)
            {
                statusCode = (HttpStatusCode)cosmosException.StatusCode;
                message = cosmosException.Message;
            }
            else if (exception is NotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
            }
            else if (exception is InvalidOperationException invalidOperationException)
            {
                statusCode = HttpStatusCode.BadRequest;
                message = invalidOperationException.Message;
            }
            // You can handle other specific exceptions here

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var errorResponse = new
            {
                message = message,
                details = exceptionType
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }

        private Task HandleForbiddenAsync(HttpContext context)
        {
            var statusCode = HttpStatusCode.Forbidden;
            var message = "You do not have permission to access this resource.";

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var errorResponse = new
            {
                message = message,
                details = "Forbidden"
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }

        private Task HandleUnauthorizedAsync(HttpContext context)
        {
            var statusCode = HttpStatusCode.Unauthorized;
            var message = "Unauthorized access.";

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var errorResponse = new
            {
                message = message,
                details = "Unauthorized"
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }

    // Define the NotFoundException class
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
