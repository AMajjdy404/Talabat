using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.API.DTOs;
using Talabat.API.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Services;

namespace Talabat.API.Controllers
{
    
  
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService paymentService;
        private readonly IMapper mapper;
        const string endpointSecret = "whsec_40eecb30863bf70526cc3343239ba5a484cb84b34eb13dd787cc0a68dab6206a";


        public PaymentsController(IPaymentService paymentService,IMapper mapper)
        {
            this.paymentService = paymentService;
            this.mapper = mapper;
        }

        // Create Or Update Payment Intent
        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
        {
           var CustumerBasket =   await paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (CustumerBasket is null) return BadRequest(new ApiResponse(400, "There is a Problem with your Basket"));
            var MappedBasket = mapper.Map<CustomerBasket, CustomerBasketDto>(CustumerBasket);
            return Ok(MappedBasket);
        }

        // WebHook
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);

                var PaymentIntent = stripeEvent.Data.Object as PaymentIntent;

                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    await paymentService.UpdatePaymentIntentStatusToSuccessOrFail(PaymentIntent.Id,false);
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    await paymentService.UpdatePaymentIntentStatusToSuccessOrFail(PaymentIntent.Id, true);

                }
                // ... handle other event types
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
    }

