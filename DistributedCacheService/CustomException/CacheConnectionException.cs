using System;

namespace DistributedCacheService.CustomException
{
    public class CacheConnectionException : Exception
    {
        public CacheConnectionException(string message) : base(message)
        {
        }

        public CacheConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
