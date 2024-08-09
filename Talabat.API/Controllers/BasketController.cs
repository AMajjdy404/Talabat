using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.DTOs;
using Talabat.API.Errors;
using Talabat.Core.Entities;
using Talabat.Core.IRepository;
using Talabat.Repository;

namespace Talabat.API.Controllers
{
    
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository basketRepository;
        private readonly IMapper mapper;

        public BasketController(IBasketRepository basketRepository,
            IMapper mapper)
        {
            this.basketRepository = basketRepository;
            this.mapper = mapper;
        }

        // Get or Re-Create Basket

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {
          var Basket = await basketRepository.GetBasketAsync(id);
            return Basket is null ? new CustomerBasket(id) : Ok(Basket);
        }

        // Create or Update Basket

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateOrCreateBasket(CustomerBasketDto Basket)
        {
            var MappedBasket = mapper.Map<CustomerBasketDto,CustomerBasket>(Basket);
           var UpdatedOrCreated = await basketRepository.CreateOrUpdateBasketAsync(MappedBasket);
            if (UpdatedOrCreated is null) return BadRequest(new ApiResponse(400));
            return Ok(UpdatedOrCreated);
        }

        // Delete Basket
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket(string BasketId)
        {
           return await basketRepository.DeleteBasketAsync(BasketId);
        }
    }
}
