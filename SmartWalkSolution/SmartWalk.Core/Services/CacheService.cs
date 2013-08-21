using System;
using System.IO;
using System.Linq;
using SmartWalk.Core.Services;

namespace SmartWalk.Core.Services
{
    public class CacheService : ICacheService
    {
        private const string CacheFileExt = ".cache";

        private readonly string _cacheFolderPath;

        public CacheService()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _cacheFolderPath = Path.Combine(documents, "..", "Library", "Caches", "SmartWalkData.Cache");
        }

        public string GetString(string key)
        {
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
            if (!Directory.Exists(_cacheFolderPath))
            {
                Directory.CreateDirectory(_cacheFolderPath);
            }

            var fileName = key + CacheFileExt;

            File.WriteAllText(Path.Combine(_cacheFolderPath, fileName), value);
        }
    }
}