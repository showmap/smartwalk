using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace SmartWalk.Client.Core.Services
{
    public class FileService : IFileService
    {
        private readonly IMvxExtendedFileStore _fileStore;
        private readonly IExceptionPolicyService _exceptionPolicy;

        public FileService(
            IMvxExtendedFileStore fileStore,
            IExceptionPolicyService exceptionPolicy)
        {
            _fileStore = fileStore;
            _exceptionPolicy = exceptionPolicy;
        }

        public string GetFileString(string path, string fileName, 
            bool deleteIfOld = false, TimeSpan? maxFileAge = null)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (fileName == null) throw new ArgumentNullException("fileName");

            if (_fileStore.FolderExists(path))
            {
                var files = _fileStore.GetFilesIn(path);
                var file = Path.Combine(path, fileName);

                string contents;

                if (files.Contains(file) &&
                    (!deleteIfOld || !DeleteIfOld(file, maxFileAge ?? TimeSpan.MaxValue)) &&
                    _fileStore.TryReadTextFile(file, out contents))
                {
                    return contents;
                }
            }

            return null;
        }

        public void SetFileString(string path, string fileName, string value)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (value == null) throw new ArgumentNullException("value");

            _fileStore.EnsureFolderExists(path);

            _fileStore.WriteFile(Path.Combine(path, fileName), value);
        }

        public void InvalidateFileString(string path, string fileName)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (fileName == null) throw new ArgumentNullException("fileName");

            _fileStore.EnsureFolderExists(path);
            _fileStore.DeleteFile(Path.Combine(path, fileName));
        }

        public T GetFileObject<T>(string path, string fileName, 
            bool deleteIfOld = false, TimeSpan? maxFileAge = null)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (fileName == null) throw new ArgumentNullException("fileName");

            var str = GetFileString(path, fileName, deleteIfOld, maxFileAge);
            var result = str != null 
                ? JsonConvert.DeserializeObject<T>(str)
                : default(T);
            return result;

        }

        public void SetFileObject<T>(string path, string fileName, T obj) where T : class
        {
            if (path == null) throw new ArgumentNullException("path");
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (obj == null) throw new ArgumentNullException("obj");

            var str = JsonConvert.SerializeObject(obj);
            SetFileString(path, fileName, str);
        }

        public void CleanUpOldFiles(string path, TimeSpan maxFileAge)
        {
            if (_fileStore.FolderExists(path))
            {
                var files = _fileStore.GetFilesIn(path);
                foreach (var file in files)
                {
                    DeleteIfOld(file, maxFileAge);
                }
            }
        }

        private bool DeleteIfOld(string file, TimeSpan maxFileAge)
        {
            var lastWriteTime = _fileStore.GetLastWriteTime(file);
            var fileAge = DateTime.Now - lastWriteTime;
            if (fileAge > maxFileAge)
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