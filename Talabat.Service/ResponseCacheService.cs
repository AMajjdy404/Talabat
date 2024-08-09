using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Services;

namespace Talabat.Service
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase _database;
        public ResponseCacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task SetCacheResponseAsync(string Key, object Response, TimeSpan TimeToLive)
        {
            var SerializeOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var SerializedResponse = JsonSerializer.Serialize(Response, SerializeOptions);
            await _database.StringSetAsync(Key, SerializedResponse, TimeToLive);
        }
        public async Task<string?> GetCachedResponseAsync(string Key)
        {
            var Response = await _database.StringGetAsync(Key);
            if(string.IsNullOrEmpty(Response)) return null;
            return Response;
        }

    }
}
