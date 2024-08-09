using System.Net;
using System.Text.Json;
using Talabat.API.Errors;

namespace Talabat.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger , IHostEnvironment env)
        {
            this.next = next;
            this.logger = logger;
            this.env = env;
        }

        // InvokeAsync
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
              await next.Invoke(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError; // 500

                if(env.IsDevelopment())
                {
                    var Response = new ApiExceptionError((int) HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace);
                    var Options = new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    var JsonResponse = JsonSerializer.Serialize(Response,Options);
                    await context.Response.WriteAsync(JsonResponse);
                    //await context.Response.WriteAsJsonAsync(Response);
                }
                else
                {
                    var Response = new ApiExceptionError((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace);
                    var Options = new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    var JsonResponse = JsonSerializer.Serialize(Response, Options);
                    await context.Response.WriteAsync(JsonResponse);
                   
                }
            }
        }
    }
}
