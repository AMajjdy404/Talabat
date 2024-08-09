using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.IRepository;

namespace Talabat.Repository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase  database;

        public BasketRepository(IConnectionMultiplexer redis)
        {
            database = redis.GetDatabase();
        }

        public async Task<bool> DeleteBasketAsync(string BasketId)
        {
            return await database.KeyDeleteAsync(BasketId);
        }
        public async Task<CustomerBasket?> GetBasketAsync(string BasketId)
        {
          var Basket =  await database.StringGetAsync(BasketId);
            return Basket.IsNull ? null : JsonSerializer.Deserialize<CustomerBasket>(Basket);
        }
        public async Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket)
        {
            var JsonBasket = JsonSerializer.Serialize(basket);
            var CreatedOrUpdated = await database.StringSetAsync(basket.Id, JsonBasket, TimeSpan.FromDays(1));
            if (!CreatedOrUpdated) return null;
            return await GetBasketAsync(basket.Id);
        }

      

    }
}
