using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DistributedCacheService.Abstractions
{
    public interface IDistributedCacheService
    {
        void AddCache<TValue>(string key, TValue value, TimeSpan? absoluteExpireTime = null);
        TValue GetCache<TValue>(string key);
        void DeleteAddCache<TValue>(string key, TValue value, TimeSpan? absoluteExpireTime = null);
        IEnumerable<RedisKey> GetAllKeys(string pattern = "*");
    }
}
