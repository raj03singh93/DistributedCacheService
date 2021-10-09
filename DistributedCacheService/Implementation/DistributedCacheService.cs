using DistributedCacheService.Abstractions;
using DistributedCacheService.Config;
using DistributedCacheService.CustomException;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DistributedCacheService.Implementation
{
    /// <summary>
    /// <inheritdoc/>
    /// <see cref="INativeCacheService"/>
    /// </summary>
    public class DistributedCacheService : IDistributedCacheService
    {
        #region Private Fields
        private IDatabase cache;
        private IServer server;
        private ConnectionMultiplexer connection;
        private readonly CacheConfigSettings cacheConfigSetting;
        #endregion

        #region Ctor
        public DistributedCacheService(CacheConfigSettings cacheConfigSetting)
        {
            this.cacheConfigSetting = cacheConfigSetting ?? throw new ArgumentNullException(nameof(cacheConfigSetting));
            Connect();
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Called to connect to redis cache
        /// </summary>
        private void Connect()
        {
            if (cache != null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(cacheConfigSetting.Host))
            {
                throw new ArgumentNullException(nameof(cacheConfigSetting.Host));
            }

            if (cacheConfigSetting.Port == null)
            {
                throw new ArgumentNullException(nameof(cacheConfigSetting.Port));
            }

            try
            {
                if (cache == null)
                {
                    string Configuration = string.IsNullOrWhiteSpace(cacheConfigSetting.Password) ? $"{cacheConfigSetting.Host}:{cacheConfigSetting.Port}" : $"{cacheConfigSetting.Host}:{cacheConfigSetting.Port},password={cacheConfigSetting.Password}";
                    connection = ConnectionMultiplexer.Connect(Configuration);
                    cache = connection.GetDatabase();
                    server = connection.GetServer(cacheConfigSetting.Host, cacheConfigSetting.Port.Value);
                }
            }
            catch (Exception e)
            {
                throw new CacheConnectionException("Error occured while Connecting to Redis cache.", e);
            }
        }
        #endregion
        #region Public Methods
        public void AddCache<TValue>(string key, TValue value, TimeSpan? absoluteExpireTime = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            string data = JsonSerializer.Serialize(value);

            cache.StringSet(key, data, absoluteExpireTime);
        }

        public TValue GetCache<TValue>(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            string data = cache.StringGet(key);
            if (data == null)
            {
                return default(TValue);
            }
            return JsonSerializer.Deserialize<TValue>(data);
        }

        public void DeleteAddCache<TValue>(string key, TValue value, TimeSpan? absoluteExpireTime = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            cache.KeyDelete(key);

            string data = JsonSerializer.Serialize(value);

            cache.StringSet(key, data, absoluteExpireTime);
        }

        public IEnumerable<RedisKey> GetAllKeys(string pattern = "*")
        {
            return server.Keys(pattern: pattern);
        }
        #endregion
    }
}
