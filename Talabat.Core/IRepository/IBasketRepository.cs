using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.IRepository
{
    public interface IBasketRepository
    {
        public Task<CustomerBasket?> GetBasketAsync(string BasketId);
        public Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket);
        public Task<bool> DeleteBasketAsync(string BasketId);
    }
}
