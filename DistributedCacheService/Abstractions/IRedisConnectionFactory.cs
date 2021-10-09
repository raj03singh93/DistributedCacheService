using DistributedCacheService.Config;

namespace DistributedCacheService.Abstractions
{
    public interface IRedisConnectionFactory
    {
        void AddRedisServer(CacheConfigSettings cacheConfigSetting);
        IDistributedCacheService GetCacheService(string redisServerName = null);
    }
}
