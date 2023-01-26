using Application.Wrapper;
using System.Net;
using System.Text.Json;

namespace WebApi.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                
                var responseModel = new Response<string>() { IsSuccess = false, Message = ex?.Message};

                switch (ex)
                {
                    case Application.Exceptions.ApiException:
                        response.StatusCode = StatusCodes.Status400BadRequest;// (int)HttpStatusCode.BadRequest;
                        break;

                    case Application.Exceptions.ValidationException e:

                        response.StatusCode = StatusCodes.Status400BadRequest;// (int)HttpStatusCode.BadRequest;
                        responseModel.Errors = e.Errors;
                        break;

                    case KeyNotFoundException:
                        response.StatusCode = StatusCodes.Status404NotFound;// (int)HttpStatusCode.NotFound;
                        break;

                    default:
                        response.StatusCode = StatusCodes.Status500InternalServerError;// (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(responseModel);

                await response.WriteAsync(result);
            }

        }
    }
}
