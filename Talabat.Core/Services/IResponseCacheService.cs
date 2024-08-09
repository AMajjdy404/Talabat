using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Services
{
    public interface IResponseCacheService
    {
        // Set Cache
        Task SetCacheResponseAsync(string Key, object Response, TimeSpan TimeToLive);

        // Get Cache
        Task<string?> GetCachedResponseAsync(string Key);
    }
}
