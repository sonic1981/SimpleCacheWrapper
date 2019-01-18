using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCacheWrapper.Imple
{
    public class CacheWrapper : ICacheWrapper
    {
        private static ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();
        private static ConcurrentDictionary<string, DateTime> _cacheExpiration = new ConcurrentDictionary<string, DateTime>();

        //unit testing only
        public void ClearCache()
        {
            _cache = new ConcurrentDictionary<string, object>();
            _cacheExpiration = new ConcurrentDictionary<string, DateTime>();
        }

            /// <summary>
            /// successive calls to this method may call populatingFunction multiple times due to race conditions.
            /// I could add a lock but didn't see the return on investment to be worthwhile
            /// </summary>
            public async Task<T> GetObjectFromCacheAsync<T>(Func<Task<T>> populatingFunction, string cacheKey, int duration = 10)
        {
            object objReturnVal;
            DateTime expiration;
            //have we an expiration stored?
            if (_cacheExpiration.TryGetValue(cacheKey, out expiration))
            {
                DateTime now = DateTime.Now;
                //If the expiration has expired or the value can't be gotten for some reason then add a new value
                //return Val is output from _cache.TryGetValue so we can just return this if both these test come back OK
                if (expiration < now ||
                    _cache.TryGetValue(cacheKey, out objReturnVal) == false)
                {

                    objReturnVal = await populatingFunction().ConfigureAwait(false);

                    _cache.AddOrUpdate(cacheKey, objReturnVal, (key, oldValue) => objReturnVal);
                    DateTime newExpr = now.AddMinutes(duration);
                    _cacheExpiration.AddOrUpdate(cacheKey, newExpr, (key, oldValue) => newExpr);
                }
            }
            else
            {
                //no expiration treat it as a fresh request
                objReturnVal = await populatingFunction().ConfigureAwait(false);
                DateTime now = DateTime.Now;

                _cache.AddOrUpdate(cacheKey, objReturnVal, (key, oldValue) => objReturnVal);

                DateTime newExpr = now.AddMinutes(duration);
                _cacheExpiration.AddOrUpdate(cacheKey, newExpr, (key, oldValue) => newExpr);
            }

            return (T)objReturnVal;
        }

        public T GetObjectFromCache<T>(Func<T> populatingFunction, string cacheKey, int duration = 10)
        {
            object objReturnVal;
            DateTime expiration;
            if (_cacheExpiration.TryGetValue(cacheKey, out expiration))
            {
                DateTime now = DateTime.Now;
                if (expiration < now || _cache.TryGetValue(cacheKey, out objReturnVal) == false)
                {

                    objReturnVal = populatingFunction();

                    _cache.AddOrUpdate(cacheKey, objReturnVal, (key, oldValue) => objReturnVal);
                    DateTime newExpr = now.AddMinutes(duration);
                    _cacheExpiration.AddOrUpdate(cacheKey, newExpr, (key, oldValue) => newExpr);
                }
            }
            else
            {
                objReturnVal = populatingFunction();
                DateTime now = DateTime.Now;

                _cache.AddOrUpdate(cacheKey, objReturnVal, (key, oldValue) => objReturnVal);

                DateTime newExpr = now.AddMinutes(duration);
                _cacheExpiration.AddOrUpdate(cacheKey, newExpr, (key, oldValue) => newExpr);
            }

            return (T)objReturnVal;
        }
    }
}
