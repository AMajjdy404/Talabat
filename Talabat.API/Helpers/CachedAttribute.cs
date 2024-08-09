using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services;

namespace Talabat.API.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveInSeconds;

        public CachedAttribute(int timeToLiveInSeconds)
        {
            _timeToLiveInSeconds = timeToLiveInSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var ResponseCacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            // Ask CLR to Create object from "ResponseCacheService" Explicitly
            var cachekey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
           var response = await ResponseCacheService.GetCachedResponseAsync(cachekey);
            if(!string.IsNullOrEmpty(response))
            {
                var Result = new ContentResult()
                {
                    Content = response,
                    ContentType = "application/json",
                    StatusCode = 200,
                };
                context.Result = Result;
                return;
            }
            // Not Cached
            var ActionExecutedContext = await next.Invoke();
            if(ActionExecutedContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null)
            {
              await  ResponseCacheService.SetCacheResponseAsync(cachekey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveInSeconds));
            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
           var KeyBuilder = new StringBuilder();
            KeyBuilder.Append(request.Path); // /api/Products
            foreach(var (key,value) in request.Query.OrderBy(x => x.Key))
            {
                KeyBuilder.Append($"|{key}-{value}");
                // /api/Products|pageindex-1
                // /api/Products|pageindex-1|PageSize-5
                // /api/Products|pageindex-1|PageSize-5|sort-name

            }

            return KeyBuilder.ToString();
        }
    }
}
