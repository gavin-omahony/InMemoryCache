using Caching.InMemoryCache;
using Newtonsoft.Json.Linq;
using System.Diagnostics.Metrics;

namespace Caching.InMemoryCacheTest
{
    [TestClass]
    public class InMemoryCacheTest
    {
        [TestMethod]
        public void Test_Caching_Object()
        {
            InMemoryCache<string> cache = InMemoryCache<string>.CreateCache(10);
            string stringToCache = "cachedString";
            cache.Add(1, stringToCache, out int? evictee);
            cache.Get(1, out string cachedItem);
            Assert.AreEqual(stringToCache, cachedItem);
        }

        [TestMethod]
        public void Test_Retrieving_Object_Not_In_Cache()
        {
            InMemoryCache<string> cache = InMemoryCache<string>.CreateCache(10);
            string stringToCache = "cachedString";
            cache.Add(1, stringToCache, out int? evictee);
            Assert.IsFalse(cache.Get(2, out string cachedItem));  
        }

        [TestMethod]
        public void Test_Add_Then_Replace()
        {
            InMemoryCache<int> cache = InMemoryCache<int>.CreateCache(10);
            int intToCache = 999;
            int? evictee = null;
            cache.Add(1, intToCache, out evictee);
            intToCache++;
            cache.Add(1, intToCache, out evictee);
            cache.Get(1, out int cachedItem);
            Assert.AreEqual(intToCache, cachedItem);
        }

        [TestMethod]
        public void Test_Eviction()
        {
            InMemoryCache<string> cache = InMemoryCache<string>.CreateCache(5);

            int? evictee = null;

            cache.Add(1, "evictee", out evictee);
            cache.Add(2, "safe", out evictee);
            cache.Add(3, "safe", out evictee);
            cache.Add(4, "safe", out evictee);
            cache.Add(5, "safe", out evictee);
            cache.Add(6, "safe", out evictee);

            Assert.AreEqual(1, evictee);
        }

        [TestMethod]
        public void Test_Parallel_Use()
        {
            InMemoryCache<string> cache = InMemoryCache<string>.CreateCache(1000);
            int? evictee = null;

            Parallel.For(0, 10, f =>
            {
                for (int i = 0; i < 100; i++)
                {
                    cache.Add(i, i.ToString(), out evictee);
                }
            });

            Parallel.For(0, 10, f =>
            {
                string value = null;

                for (int i = 0; i < 100; i++)
                {
                    cache.Get(i, out value);
                    Assert.AreEqual(i.ToString(), value);
                }
            });
        }

        [TestMethod]
        public void Test_Parallel_Instantiation()
        {
            var cacheArray = new InMemoryCache<string>[15];

            Parallel.For(1, 15, f =>
            {
                cacheArray[f] = InMemoryCache<string>.CreateCache(1000);
                cacheArray[f].Add(f, f.ToString(), out int? evictee);
            });

            for (int i = 1; i < 15; i++)
            {
                cacheArray[10].Get(i, out string? value);
                Assert.AreEqual(i.ToString(), value);
            }
        }
    }
}