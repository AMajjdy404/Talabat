using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggreagate;

namespace Talabat.Core.Services
{
    public interface IOrderService
    {
        public Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress);
        public Task<IReadOnlyList<Order>> GetOrdersForSpecificUser(string buyerEmail);
        public Task<Order> GetOrderByIdForSpecificUser(string buyerEmail, int orderId);

    }
}
