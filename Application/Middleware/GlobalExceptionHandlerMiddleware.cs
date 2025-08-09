using Application.ApiResponse;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Text.Json;




namespace ApiVault.MiddleWare
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";

                var statusCode = ex switch
                {
                    UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                    KeyNotFoundException => HttpStatusCode.NotFound,
                    ArgumentException => HttpStatusCode.BadRequest,
                    _ => HttpStatusCode.InternalServerError
                };

                context.Response.StatusCode = (int)statusCode;

                // Get role claim if available
                string role = context.User?.Claims?.FirstOrDefault(c => c.Type == "role")?.Value;

                ApiResponse<object> apiResponse;

                if (_env.IsDevelopment() || role == "Admin")
                {
                    // Detailed error for dev or Admin
                    var errorDetails = new
                    {
                        Exception = ex.GetType().Name,
                        StackTrace = ex.StackTrace,
                        Error = ex.Message
                    };

                    apiResponse = new ApiResponse<object>
                    {
                        StatusCode = (int)statusCode,
                        Message = ex.Message,
                        Data = errorDetails
                    };
                }
                else if (role == "User")
                {
                    apiResponse = ApiResponse<object>.Fail("An error occurred while processing your request.", (int)statusCode);
                }
                else
                {
                    // For anonymous or unknown roles
                    apiResponse = ApiResponse<object>.Fail("Unauthorized access or internal server error.", (int)statusCode);
                }

                var json = JsonSerializer.Serialize(apiResponse);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
