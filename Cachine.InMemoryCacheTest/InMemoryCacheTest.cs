using System;
using System.Net.Http.Headers;
using Caching.InMemoryCache;

namespace Caching.InMemoryCacheTest
{
    [TestClass]
    public class InMemoryCacheTest<TValue>
    {
        [TestMethod]
        public void TestCachingObject()
        {
            InMemoryCache<string> cache = InMemoryCache<string>.CreateCache(10);
            string stringToCache = "CacheToString";
            cache.Add(1, stringToCache, out int? evictee);
            cache.Get(1, out string cachedItem);
            Assert.AreEqual(stringToCache, cachedItem);
        }

        [TestMethod]
        public void TestAddThenReplace()
        {
            InMemoryCache<int> cache = InMemoryCache<int>.CreateCache(10);
            int intToCache = 999;

            AddItemToCache(cache, 1, intToCache);

            intToCache++;

            AddItemToCache(cache, 1, intToCache);

            cache.Get(1, out int cachedItem);

            Assert.AreEqual(intToCache, cachedItem);
        }

        static public void AddItemToCache(InMemoryCache<int> cache, int key, object value)
        {
            cache.Add(key, value, out int? evictee);

            if (evictee != null)
            {
                //Do something
            }
        }
    }
}