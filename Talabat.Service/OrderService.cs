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
using Talabat.Core.Specifications.OrderSpec;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository basketRepo;
        private readonly IUnitOfWork unitOfWork;
        private readonly IPaymentService paymentService;

        public OrderService(IBasketRepository basketRepo,
          IUnitOfWork unitOfWork,
          IPaymentService paymentService)
        {
            this.basketRepo = basketRepo;
            this.unitOfWork = unitOfWork;
            this.paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            // 1.Get Basket From Basket Repo

            var Basket = await basketRepo.GetBasketAsync(basketId);

            // 2.Get Selected Items at Basket From Product Repo

            var OrderItems = new List<OrderItem>();
            if(Basket?.Items.Count > 0)
            {
                foreach(var item in Basket.Items)
                {
                    var Product = await unitOfWork.Repository<Product>().GeyByIdAsync(item.Id);
                    var ProductItemOrdered = new ProductItemOrdered(Product.Id,Product.Name,Product.PictureUrl);
                    var OrderItem = new OrderItem(ProductItemOrdered, Product.Price, item.Quantity);
                    OrderItems.Add(OrderItem);
                }
            }

            // 3.Calculate SubTotal => item.price * item.quantity

            var SubTotal = OrderItems.Sum(items => items.Price * items.Quantity);

            // 4.Get Delivery Method From DeliveryMethod Repo

            var DeliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GeyByIdAsync(deliveryMethodId);

            // 5.Create Order
            var Spec = new OrderWithPaymentIntentSpec(Basket.PaymentIntentId);
            var ExOrder = await unitOfWork.Repository<Order>().GetEntitylWithSpecAsync(Spec);
            if(ExOrder is not null)
            {
                unitOfWork.Repository<Order>().Delete(ExOrder);
                await paymentService.CreateOrUpdatePaymentIntent(basketId);
            }

            var Order = new Order(buyerEmail, shippingAddress, DeliveryMethod, OrderItems, SubTotal,Basket.PaymentIntentId);

            // 6.Add Order Locally

            await unitOfWork.Repository<Order>().AddAsync(Order);

            // 7.Save Order To Database[ToDo]
            var Result = await unitOfWork.CompleteAsync();
            if (Result <= 0) return null;
            
            return Order;


        }

        public async Task<Order> GetOrderByIdForSpecificUser(string buyerEmail, int orderId)
        {
            var Spec = new OrderSpecifications(buyerEmail, orderId);
            var Order = await unitOfWork.Repository<Order>().GetEntitylWithSpecAsync(Spec);
            return Order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForSpecificUser(string buyerEmail)
        {
            var Spec = new OrderSpecifications(buyerEmail);
            var Orders = await unitOfWork.Repository<Order>().GetAllWithSpecAsync(Spec);
            return Orders;
        }
    }
}
