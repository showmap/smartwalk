using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SmartWalk.Client.Core.Services;

namespace SmartWalk.Client.iOS.Services
{
    public class CacheService : ICacheService
    {
        private const string CacheFileExt = ".json";

        private readonly string _cacheFolderPath;

        public CacheService()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _cacheFolderPath = Path.Combine(documents, "..", "Library", "Caches", "SmartWalkData.Cache");
        }

        public string GetString(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            if (Directory.Exists(_cacheFolderPath))
            {
                var files = Directory.EnumerateFiles(_cacheFolderPath).ToArray();
                var fileName = key + CacheFileExt;
                var file = Path.Combine(_cacheFolderPath, fileName);

                if (files.Contains(file))
                {
                    return File.ReadAllText(file);
                }
            }

            return null;
        }

        public void SetString(string key, string value)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");

            if (!Directory.Exists(_cacheFolderPath))
            {
                Directory.CreateDirectory(_cacheFolderPath);
            }

            var fileName = key + CacheFileExt;

            File.WriteAllText(Path.Combine(_cacheFolderPath, fileName), value);
        }

        public T GetObject<T>(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            var str = GetString(key);
            var result = str != null 
                ? JsonConvert.DeserializeObject<T>(str)
                : default(T);
            return result;
        }

        public void SetObject<T>(string key, T obj)
            where T : class
        {
            if (key == null) throw new ArgumentNullException("key");
            if (obj == null) throw new ArgumentNullException("obj");

            var str = JsonConvert.SerializeObject(obj);
            SetString(key, str);
        }
    }
}