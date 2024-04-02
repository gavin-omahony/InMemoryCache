using Caching.InMemoryCache;

InMemoryCache<Int32> cache = InMemoryCache<Int32>.CreateCache(10);


Console.WriteLine("whats up");

cache.Get(15, out Int32 value);
cache.Add(9, 9, out int? evictee);