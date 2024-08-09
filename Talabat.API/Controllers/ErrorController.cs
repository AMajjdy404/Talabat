using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.Errors;

namespace Talabat.API.Controllers
{
    [Route("error/{StatusCode}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        public ActionResult NotFoundEndPoint(int StatusCode)
        {
            return NotFound(new ApiResponse(StatusCode));
        }
    }
}
