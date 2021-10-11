using DistributedCacheService.Abstractions;
using DistributedCacheService.Config;
using DistributedCacheService.CustomException;
using DistributedCacheService.Implementation;
using System;
using Xunit;

namespace NativeCacheServiceTest
{
    public class DistributedCacheServiceUnitTest
    {
        private readonly CacheConfigSettings configSetting;
        private readonly CacheConfigSettings configSettingSec;
        public DistributedCacheServiceUnitTest()
        {
            configSetting = new CacheConfigSettings();
            configSetting.Host = "localhost";
            configSetting.Port = 6379;
            configSetting.Name = "kickbox";

            configSettingSec = new CacheConfigSettings();
            configSettingSec.Host = "localhost";
            configSettingSec.Port = 6479;
            configSettingSec.Name = "expiry";
        }
        [Fact(DisplayName = "Missing Configuration")]
        public void ConfigurationNull_Test()
        {

            // Arrange, Act and Assert
            Assert.Throws<CacheConnectionException>(() => new DistributedCacheService.Implementation.DistributedCacheService(new CacheConfigSettings()));

        }

        [Fact(DisplayName = "Add New Item To Cache")]
        public void AddNewItemToCache_Test()
        {
            // Arrange
            IDistributedCacheService cache = new DistributedCacheService.Implementation.DistributedCacheService(configSetting);

            // Act
            cache.AddCache("key1", "value1");

            // Assert
            Assert.True(cache.GetCache<string>("key1").Equals("value1"), "Failed to get item from cache");
        }

        [Theory(DisplayName = "Throw null if key/value missing")]
        [InlineData(null, null)]
        [InlineData(null, "value")]
        [InlineData("key", null)]
        public void AddCache_WithNullKeyValue_Test(string key, string value)
        {
            // Arrange
            IDistributedCacheService cache = new DistributedCacheService.Implementation.DistributedCacheService(configSetting);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => cache.AddCache(key, value));
        }

        [Fact(DisplayName = "Add Complex Datatype To Cache")]
        public void AddCache_ComplexDatatype_Test()
        {
            // Arrange
            IDistributedCacheService cache = new DistributedCacheService.Implementation.DistributedCacheService(configSetting);


            // Act
            cache.AddCache("key1", new ComplexData() { Id = 1, Name = "Sam" });

            // Assert
            ComplexData data = cache.GetCache<ComplexData>("key1");
            Assert.True(data.Name.Equals("Sam"), "Failed to get item from cache");
        }

        [Fact(DisplayName = "Cache factory")]
        public void CacheFactory_CreateMultipleCache()
        {
            // Arrange
            IRedisConnectionFactory factory = new RedisConnectionFactory();
            factory.AddRedisServer(configSetting);
            factory.AddRedisServer(configSettingSec);

            var cache = factory.GetCacheService(configSetting.Name);
            var expiry = factory.GetCacheService(configSettingSec.Name);

            // Act
            cache.AddCache("key1", new ComplexData() { Id = 1, Name = "Sam" });
            ComplexData data = cache.GetCache<ComplexData>("key1");

            expiry.AddCache("key2", data);
            ComplexData data2 = expiry.GetCache<ComplexData>("key2");

            // Assert
            Assert.Equal(data2.Name, data.Name);
        }
        private class ComplexData
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
