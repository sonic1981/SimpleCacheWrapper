using System;
using System.Threading.Tasks;

namespace SimpleCacheWrapper
{
    public interface ICacheWrapper
    {
        T GetObjectFromCache<T>(Func<T> populatingFunction, string cacheKey, int duration = 10);
        Task<T> GetObjectFromCacheAsync<T>(Func<Task<T>> populatingFunction, string cacheKey, int duration = 10);
    }
}