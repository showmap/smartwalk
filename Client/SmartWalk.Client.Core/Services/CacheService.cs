using System;
using System.Threading.Tasks;
using SmartWalk.Client.Core.Utils;

namespace SmartWalk.Client.Core.Services
{
    public class CacheService : ICacheService
    {
        private const string CacheFileExt = ".json";

        private readonly IConfiguration _configuration;
        private readonly IMvxExtendedFileStore _fileStore;
        private readonly IFileService _fileService;
        private readonly string _cacheFolderPath;

        public CacheService(
            IConfiguration configuration, 
            IMvxExtendedFileStore fileStore,
            IFileService fileService)
        {
            _configuration = configuration;
            _fileStore = fileStore;
            _fileService = fileService;
            _cacheFolderPath = _fileStore.NativePath(
                _configuration.CacheConfig.CacheFolderPath);

            // cleaning old stuff, 2 months expiration
            Task.Run(() => _fileService.CleanUpOldFiles(_cacheFolderPath, TimeSpan.FromDays(60)))
                .ContinueWithThrow();
        }

        public string GetString(string key, bool deleteIfOld = true)
        {
            return _fileService.GetFileString(_cacheFolderPath, key + CacheFileExt, 
                deleteIfOld, _configuration.CacheConfig.MaxFileAge);
        }

        public void SetString(string key, string value)
        {
            _fileService.SetFileString(_cacheFolderPath, key + CacheFileExt, value);
        }

        public void InvalidateString(string key)
        {
            _fileService.InvalidateFileString(_cacheFolderPath, key + CacheFileExt);
        }

        public T GetObject<T>(string key, bool deleteIfOld = true)
        {
            return _fileService.GetFileObject<T>(_cacheFolderPath, key + CacheFileExt, 
                deleteIfOld, _configuration.CacheConfig.MaxFileAge);
        }

        public void SetObject<T>(string key, T obj)
            where T : class
        {
            _fileService.SetFileObject<T>(_cacheFolderPath, key + CacheFileExt, obj);
        }
    }
}