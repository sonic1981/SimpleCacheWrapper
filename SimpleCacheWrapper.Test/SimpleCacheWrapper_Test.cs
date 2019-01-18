using Moq;
using NUnit.Framework;
using SimpleCacheWrapper.Imple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCacheWrapper.Test
{
    [TestFixture]
    public class SimpleCacheWrapper_Test
    {
        [Test]
        public async Task LinearTest()
        {


            CacheWrapper cacheMan = new CacheWrapper();
            cacheMan.ClearCache();

            int result = await cacheMan.GetObjectFromCacheAsync(() => testGet1(), "test");
            await Task.Delay(2000);
            result = await cacheMan.GetObjectFromCacheAsync(() => testGet2(), "test");

            Assert.AreEqual(1, result);
        }

        [Test]
        public async Task AsyncTest()
        {


            CacheWrapper cacheMan = new CacheWrapper();
            cacheMan.ClearCache();

            List<Task<int>> tasks = new List<Task<int>>();
            tasks.Add(cacheMan.GetObjectFromCacheAsync(() => testGet1(), "test"));
            tasks.Add(cacheMan.GetObjectFromCacheAsync(() => testGet2(), "test"));

            await Task.WhenAll(tasks);

            int afterResult = await cacheMan.GetObjectFromCacheAsync(() => testGet3(), "test");

            //This is expected as no locking
            Assert.AreEqual(1, tasks[0].Result, "First resault incorrect");
            Assert.AreEqual(2, tasks[1].Result, "Second result incorrect");

            //resultds once cache is completed are consistent
            Assert.AreNotEqual(3, afterResult, "After result incorrect");
        }


        [Test]
        public async Task TestExpiration()
        {

            CacheWrapper cacheMan = new CacheWrapper();
            cacheMan.ClearCache();

            int result = await cacheMan.GetObjectFromCacheAsync(() => testGet1(), "test", 1);
            await Task.Delay(62000);
            result = await cacheMan.GetObjectFromCacheAsync(() => testGet2(), "test", 1);

            Assert.AreEqual(2, result);
        }


        private async Task<int> testGet1()
        {
            await Task.Delay(1000);

            return await Task.FromResult(1);
        }

        private async Task<int> testGet2()
        {
            await Task.Delay(1000);

            return await Task.FromResult(2);
        }
        private async Task<int> testGet3()
        {
            await Task.Delay(1000);

            return await Task.FromResult(3);
        }
    }
}
