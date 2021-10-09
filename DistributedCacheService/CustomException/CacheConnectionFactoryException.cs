using System;

namespace DistributedCacheService.CustomException
{
    public class CacheConnectionFactoryException : Exception
    {
        public CacheConnectionFactoryException(string message) : base(message)
        {
        }

        public CacheConnectionFactoryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
