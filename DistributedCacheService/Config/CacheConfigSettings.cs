namespace DistributedCacheService.Config
{
    public class CacheConfigSettings
    {
        public string Host { get; internal set; }
        public int? Port { get; internal set; }
        public string Password { get; internal set; }
        public string Name { get; internal set; }
    }
}
