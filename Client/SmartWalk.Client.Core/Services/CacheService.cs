using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartWalk.Client.Core.Utils;

namespace SmartWalk.Client.Core.Services
{
    public class CacheService : ICacheService
    {
        private const string CacheFileExt = ".json";

        private readonly IConfiguration _configuration;
        private readonly IMvxExtendedFileStore _fileStore;
        private readonly IExceptionPolicyService _exceptionPolicy;
        private readonly string _cacheFolderPath;

        public CacheService(
            IConfiguration configuration, 
            IMvxExtendedFileStore fileStore,
            IExceptionPolicyService exceptionPolicy)
        {
            _configuration = configuration;
            _fileStore = fileStore;
            _exceptionPolicy = exceptionPolicy;
            _cacheFolderPath = _fileStore.NativePath(
                _configuration.CacheConfig.CacheFolderPath);

            Task.Run((Action)CleanUpOldFiles).ContinueWithThrow();
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
                    !DeleteIfOld(file) &&
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

        public void InvalidateString(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            _fileStore.EnsureFolderExists(_cacheFolderPath);

            var fileName = key + CacheFileExt;

            _fileStore.DeleteFile(Path.Combine(_cacheFolderPath, fileName));

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

        private void CleanUpOldFiles()
        {
            if (_fileStore.FolderExists(_cacheFolderPath))
            {
                var files = _fileStore.GetFilesIn(_cacheFolderPath);
                foreach (var file in files)
                {
                    DeleteIfOld(file);
                }
            }
        }

        private bool DeleteIfOld(string file)
        {
            var lastWriteTime = _fileStore.GetLastWriteTime(file);
            var fileAge = DateTime.Now - lastWriteTime;
            if (fileAge > _configuration.CacheConfig.MaxFileAge)
            {
                try
                {
                    _fileStore.DeleteFile(file);
                    return true;
                }
                catch (Exception ex)
                {
                    _exceptionPolicy.Trace(ex, false);
                }
            }

            return false;
        }
    }
}