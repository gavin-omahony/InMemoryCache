# Least Recently Used In Memory Cache

## Usage
**Creating the cache:**
The cache can only be created through the static CreateCache(sizelimit) method. The type of object to be cached and the max number of items allowed in the cache must be specified as below:
```
    InMemoryCache<string> cache = InMemoryCache<string>.CreateCache(10);
```

Note: calling CreateCache() with an updated cache size or object type will create a fresh instance, your cached objects will be lost.

**Caching an object:**
Objects are cached using the Add(key, value) method. This method returns nullable int as an out paramater. This out parameter contains the key of the evicted object if the size limit has been breached.
```
    cache.Add(1, "objectToBeCached", out int? evictee);
    if (evictee.HasValue)
    {
        //Do something
    }
```
**Retrieving a cached object:**
Cached objects are retrieved using the Get(key) method. This method returns a bool indicating if the retrieval was successful. The cached object is returned in an out parameter. If the object was not found in the cache, this out paramater will contian the default value for that type.
```
    if (cache.Get(1, out string value))
    {
        //Do something
    }
```
