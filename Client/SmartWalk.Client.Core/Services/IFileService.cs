using System;

namespace SmartWalk.Client.Core.Services
{
    public interface IFileService
    {
        string GetFileString(string path, string fileName, 
            bool deleteIfOld = false, TimeSpan? maxFileAge = null);

        void SetFileString(string path, string fileName, string value);

        void InvalidateFileString(string path, string fileName);

        T GetFileObject<T>(string path, string fileName, 
            bool deleteIfOld = false, TimeSpan? maxFileAge = null);

        void SetFileObject<T>(string path, string fileName, T obj) where T : class;

        void CleanUpOldFiles(string path, TimeSpan maxFileAge);
    }
}