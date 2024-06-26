﻿namespace Caching.InMemoryCache
{
    /// <summary>
    /// Least recently used in-memory cache implementation.
    /// </summary>
    /// <typeparam name="TValue">Type of object to be cached.</typeparam>
    public sealed class InMemoryCache<TValue>
    {
        /// <summary>
        /// Max number of objects allowed in the cache.
        /// </summary>
        private readonly int _maxCachedItems;

        /// <summary>
        /// Object used to restrict access to the cache during read/write.
        /// </summary>
        private static readonly object cacheLock = new object();

        /// <summary>
        /// Object used to restrict access to the constructor.
        /// </summary>
        private static readonly object instanceLock = new object();

        /// <summary>
        /// The active instance of the class.
        /// </summary>
        private static InMemoryCache<TValue>? instance = null;

        /// <summary>
        /// Dictionary containing the cached items keys and a tuple (LinkedListNode key, value)
        /// </summary>
        private readonly Dictionary<int, (LinkedListNode<int> key, TValue value)> cache = new();

        /// <summary>
        /// Linked list with each node containing the cached items key.
        /// </summary>
        private readonly LinkedList<int> ordered = new();

        /// <summary>
        /// Creates a private instance of the InMemoryCache class.
        /// </summary>
        /// <param name="maxCachedItems">Max number of objects the cache can hold.</param>
        /// <remarks>Only called under lock to ensure a single instance is used.</remarks>
        private InMemoryCache(int maxCachedItems)
        {
            _maxCachedItems = maxCachedItems;
        }

        /// <summary>
        /// Creates and/or returns the single instance of the cache.
        /// </summary>
        /// <param name="maxCachedItems">Max number of objects the cache can hold.</param>
        /// <returns>The instance of InMemoryCache</returns>
        public static InMemoryCache<TValue> CreateCache(int maxCachedItems)
        {
            if (maxCachedItems < 1)
                throw new ArgumentException("Cache size limit must be greater than 0.");

            lock (instanceLock) 
            { 
                if (instance == null || maxCachedItems != instance._maxCachedItems || typeof(TValue) != instance.GetType().GenericTypeArguments[0])
                {
                    instance = new InMemoryCache<TValue>(maxCachedItems);
                }
            }

            return instance;
        }

        /// <summary>
        /// Attempts to retrieve the cached object for given key.
        /// </summary>
        /// <param name="key">Key of the cached item.</param>
        /// <returns>Bool indicating if the object was found or not. Value of the object if found, default or null if not found.</returns>
        public bool Get(int key, out TValue? value)
        {
            lock (cacheLock)
            {
                if (cache.TryGetValue(key, out var cachedValue))
                {
                    ordered.Remove(cachedValue.key);
                    ordered.AddLast(cachedValue.key);
                    value = cachedValue.value;
                    return true;
                }
                else
                {
                    value = default(TValue);
                }
            }

            return false;
        }

        /// <summary>
        /// Adds an object to the cache. Evicts the least recently used item if the cache is full.
        /// </summary>
        /// <param name="key">Key of the item to be cached.</param>
        /// <param name="value">Value to be cached.</param>
        /// <returns>The evicted items Key if the size limit was breached. Null if nothing was evicted.</returns>
        public void Add(int key, TValue value, out int? evictee)
        {
            evictee = null;

            lock (cacheLock)
            { 
                if (cache.TryGetValue(key, out var cachedValue))
                {
                    cache[key] = (cachedValue.key, value);
                    ordered.Remove(cachedValue.key);
                    ordered.AddLast(cachedValue.key);
                }
                else
                {
                    ordered.AddLast(key);
                    cache.Add(key, (ordered.Last!,  value));                   
                }
                if (cache.Count > _maxCachedItems)
                {
                    evictee = ordered.First!.Value;
                    cache.Remove(ordered.First!.Value);
                    ordered.RemoveFirst(); 
                }
            }
        }
    }
}
