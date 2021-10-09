using DistributedCacheService.Abstractions;
using DistributedCacheService.Config;
using DistributedCacheService.CustomException;
using System;
using System.Collections.Generic;

namespace DistributedCacheService.Implementation
{
    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        private const string defaultServer = "default";
        private Dictionary<string, IDistributedCacheService> cacheServers;

        public RedisConnectionFactory()
        {
            cacheServers = new Dictionary<string, IDistributedCacheService>();
        }

        public void AddRedisServer(CacheConfigSettings cacheConfigSetting)
        {
            if (cacheServers.Count == 0 && string.IsNullOrWhiteSpace(cacheConfigSetting.Name))
            {
                cacheConfigSetting.Name = defaultServer;
            }

            if (string.IsNullOrWhiteSpace(cacheConfigSetting.Name))
            {
                throw new ArgumentNullException(nameof(cacheConfigSetting.Name));
            }

            if (cacheServers.ContainsKey(cacheConfigSetting.Name))
            {
                throw new CacheConnectionFactoryException("Duplicate servers not allowed.");
            }

            cacheServers.Add(cacheConfigSetting.Name, new DistributedCacheService(cacheConfigSetting));

        }

        public IDistributedCacheService GetCacheService(string redisServerName = null)
        {
            if (cacheServers.Count == 1 && string.IsNullOrWhiteSpace(redisServerName))
            {
                redisServerName = defaultServer;
            }
            if (string.IsNullOrWhiteSpace(redisServerName))
            {
                throw new ArgumentNullException(nameof(redisServerName));
            }

            if (!cacheServers.ContainsKey(redisServerName))
            {
                throw new CacheConnectionFactoryException("No such cache server found.");
            }
            return cacheServers[redisServerName];
        }
    }
}
