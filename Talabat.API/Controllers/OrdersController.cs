using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.API.DTOs;
using Talabat.API.Errors;
using Talabat.Core;
using Talabat.Core.Entities.Order_Aggreagate;
using Talabat.Core.Services;

namespace Talabat.API.Controllers
{
    
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService orderService;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public OrdersController(IOrderService orderService,IMapper mapper,IUnitOfWork unitOfWork)
        {
            this.orderService = orderService;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }
        // Create Order
        [ProducesResponseType(typeof(Order),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status400BadRequest)]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var MappedAddress = mapper.Map<AddressDto, Address>(orderDto.shipToAddress);
            var Order = await orderService.CreateOrderAsync(BuyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, MappedAddress);
            if (Order is null) return BadRequest(new ApiResponse(400, "There is a Problem with your Order"));
            return Ok(Order);
        }

        // Get User Orders
        [ProducesResponseType(typeof(IReadOnlyList<OrderToReturnDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetUserOrders()
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var Orders = await orderService.GetOrdersForSpecificUser(BuyerEmail);
            if (Orders is null) return NotFound(new ApiResponse(404, "This User Has No Orders"));
            var MappedOrder = mapper.Map<IReadOnlyList<Order>,IReadOnlyList<OrderToReturnDto>>(Orders);
            return Ok(MappedOrder);
        }

        // Get Order By Id for Specific User
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{OrderId}")]
        [Authorize]
        public async Task<ActionResult<Order>> GetOrderByIdForUser(int OrderId)
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var Order = await orderService.GetOrderByIdForSpecificUser(BuyerEmail, OrderId);
            if (Order is null) return NotFound(new ApiResponse(404, $"There is No Order With Id {OrderId} For This User"));
            return Ok(Order);
        }

        // Get All DeliveryMethods
        [HttpGet("DeliveryMethods")]
        public async Task<ActionResult<DeliveryMethod>> GetAllDeliveryMethods()
        {
            var DeliveryMethods = await unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            return Ok(DeliveryMethods);
        }
    }
}
