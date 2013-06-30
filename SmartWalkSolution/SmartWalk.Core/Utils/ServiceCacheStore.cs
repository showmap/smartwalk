using System;
using System.Collections.Generic;

namespace SmartWalk.Core.Utils
{
    /// <summary>
    /// The util class that is used to simplify the caching of the service's data.
    /// </summary>
    /// <typeparam name="T">The type of the server results.</typeparam>
    public class ServiceCacheStore<T> where T : class
    {
        private readonly object _lock = new object();

        private readonly Dictionary<object, T> _cache = 
            new Dictionary<object, T>();

        private readonly Dictionary<object, bool> _isInProgress = 
            new Dictionary<object, bool>();

        private readonly Dictionary<object, List<Action<T, Exception>>> _resultHandlers = 
            new Dictionary<object, List<Action<T, Exception>>>();

        public Dictionary<object, T>.KeyCollection Keys
        {
            get
            {
                return _cache.Keys;
            }
        }

        /// <summary>
        /// Gets the cached items if it's saved before. Loads the data from server if there is no cache available yet.
        /// </summary>
        /// <param name="source">Sets the source of the items. If it's need to reload items mandatory then use DataStoreSource.Service option.</param>
        /// <param name="key">The custom parameter to distinguish result values. Can be Null.</param>
        /// <param name="completedHandler">Setups the service's Completed event listener.</param>
        /// <param name="resultHandler">Sets the result handler to invoke when everything is ready.</param>
        public void GetCachedItems(DataStoreSource source,
                                   object key,
                                   Action<Action<T, Exception>> completedHandler,
                                   Action<T, Exception> resultHandler)
        {
            if (completedHandler == null) throw new ArgumentNullException("completedHandler");
            if (resultHandler == null) throw new ArgumentNullException("resultHandler");

            // if key is null then use any value to avoid exception in Dictionary
            key = key ?? 0;

            lock (_lock)
            {
                // if there is no cached value then load it from server
                if (!_cache.ContainsKey(key) || source == DataStoreSource.FromService)
                {
                    // if the request for the key is invoked then put result handler in queue
                    if (_isInProgress.ContainsKey(key))
                    {
                        if (!_resultHandlers.ContainsKey(key))
                        {
                            _resultHandlers[key] = new List<Action<T, Exception>>();
                        }

                        _resultHandlers[key].Add(resultHandler);
                    }
                    else
                    {
                        // setup the server request's complete handler and request server
                        completedHandler((items, ex) =>
                                         {
                            lock (_lock)
                            {
                                _cache[key] = ex != null ? default(T) : items;

                                resultHandler(_cache[key], ex);

                                // if there are handlers in queue 
                                // then it's the time to give them a value
                                if (_resultHandlers.ContainsKey(key))
                                {
                                    _resultHandlers[key].ForEach(a => a(_cache[key], ex));
                                    _resultHandlers.Remove(key);
                                }

                                _isInProgress.Remove(key);
                            }
                        });

                        _isInProgress[key] = true;
                    }
                }
                else
                {
                    // TODO: To use real Clone here
                    //resultHandler(_cache[key].Clone(), null);

                    resultHandler(_cache[key], null);
                }
            }
        }

        public void Reset()
        {
            _cache.Clear();
        }

        public void ResetKey(object key)
        {
            _cache.Remove(key);
        }
    }

    public enum DataStoreSource
    {
        FromCache,
        FromService
    }
}