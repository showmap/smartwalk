using System;
using System.IO;
using System.Linq;
using Cirrious.MvvmCross.Plugins.File;
using Newtonsoft.Json;

namespace SmartWalk.Client.Core.Services
{
    public class CacheService : ICacheService
    {
        private const string CacheFileExt = ".json";

        private readonly IConfiguration _configuration;
        private readonly IMvxFileStore _fileStore;
        private readonly string _cacheFolderPath;

        public CacheService(IConfiguration configuration, IMvxFileStore fileStore)
        {
            _configuration = configuration;
            _fileStore = fileStore;
            _cacheFolderPath = _fileStore.NativePath(_configuration.CacheFolderPath);
        }

        public string GetString(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            if (_fileStore.FolderExists(_cacheFolderPath))
            {
                var files = _fileStore.GetFilesIn(_cacheFolderPath);
                var fileName = key + CacheFileExt;
                var file = Path.Combine(_cacheFolderPath, fileName);

                string contents;

                if (files.Contains(file) &&
                    _fileStore.TryReadTextFile(file, out contents))
                {
                    return contents;
                }
            }

            return null;
        }

        public void SetString(string key, string value)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");

            _fileStore.EnsureFolderExists(_cacheFolderPath);

            var fileName = key + CacheFileExt;

            _fileStore.WriteFile(Path.Combine(_cacheFolderPath, fileName), value);
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