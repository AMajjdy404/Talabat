using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggreagate;
using Talabat.Core.IRepository;
using Talabat.Core.Services;
using Talabat.Core.Specifications;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository basketRepository;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;

        public PaymentService(IBasketRepository basketRepository,
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            this.basketRepository = basketRepository;
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
        }
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string BasketId)
        {
            // Secret Key
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

            // Get Basket
            var Basket = await basketRepository.GetBasketAsync(BasketId);
            if (Basket is null) return null;

            // we need subTotal + shippingPrice
            var ShippingCost = 0M;
            if (Basket.DeliveryMethodId.HasValue)
            {
                var DeliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GeyByIdAsync(Basket.DeliveryMethodId.Value);
                ShippingCost = DeliveryMethod.Cost;
            }
            if(Basket.Items.Count > 0)
            {
                foreach (var item in Basket.Items)
                {
                    var Product = await unitOfWork.Repository<Product>().GeyByIdAsync(item.Id);
                    if(item.Price != Product.Price)
                        item.Price = Product.Price;
                }
            }

            var SubTotal = Basket.Items.Sum(item => item.Price * item.Quantity);

            // Create Payment Intent

            var Service = new PaymentIntentService();
            PaymentIntent paymentIntent;
            if (string.IsNullOrEmpty(Basket.PaymentIntentId)) // Create
            {
                var Options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)(SubTotal * 100 + ShippingCost * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };
                paymentIntent =await Service.CreateAsync(Options);
                Basket.PaymentIntentId = paymentIntent.Id;
                Basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else // Update
            {
                var Options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)(SubTotal * 100 + ShippingCost * 100)
                };
                paymentIntent = await Service.UpdateAsync(Basket.PaymentIntentId, Options);
                Basket.PaymentIntentId = paymentIntent.Id;
                Basket.ClientSecret = paymentIntent.ClientSecret;
            }
            await basketRepository.CreateOrUpdateBasketAsync(Basket);
            return Basket;

        }

        public async Task<Order> UpdatePaymentIntentStatusToSuccessOrFail(string paymentIntentId, bool flag)
        {
            var spec = new OrderWithPaymentIntentSpec(paymentIntentId);
            var Order = await unitOfWork.Repository<Order>().GetEntitylWithSpecAsync(spec);
            if (flag)
            {
                Order.Status = OrderStatus.PaymentReceived;
            }
            else
            {
                Order.Status = OrderStatus.PaymentFailed;
            }
            unitOfWork.Repository<Order>().Update(Order);
            await unitOfWork.CompleteAsync();
            return Order;
        }
    }
}
