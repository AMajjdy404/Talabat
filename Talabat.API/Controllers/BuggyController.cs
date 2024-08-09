using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.Errors;
using Talabat.Repository.Data;

namespace Talabat.API.Controllers
{

    public class BuggyController : BaseApiController
    {
        private readonly StoreContext context;

        public BuggyController(StoreContext context)
        {
            this.context = context;
        }

        // Not Found Error
        [HttpGet("NotFound")]
        public ActionResult GetNotFoundRequest()
        {
            var Product = context.Products.Find(100);
            if(Product is null)
                return NotFound(new ApiResponse(404));
            return Ok(Product);
        }

        // Server Error
        [HttpGet("ServerError")]
        public ActionResult GetServerError()
        {
            var Product = context.Products.Find(100);
            var ProductToReturn = Product.ToString(); // Error => Will Throw Null Reference Exception
            return Ok(ProductToReturn);
        }

        // Bad Request
        [HttpGet("BadRequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest();
        }

        // Validation Error
        [HttpGet("BadRequest/{id}")]
        public ActionResult GetBadRequest(int id)
        {
            return Ok();
        }
        
    }
}
