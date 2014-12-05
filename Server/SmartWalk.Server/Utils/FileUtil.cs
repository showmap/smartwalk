using System;
using System.IO;
using System.IO.IsolatedStorage;
using Orchard.FileSystems.Media;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Utils
{
    public static class FileUtil
    {
        private const string ImageUploadsDir = "ImageUploads";

        public static IsolatedStorageFile GetUploadedImageStorage()
        {
            var storage = IsolatedStorageFile.GetUserStoreForAssembly();
            if (!storage.DirectoryExists(ImageUploadsDir))
            {
                storage.CreateDirectory(ImageUploadsDir);
            }

            return storage;
        }

        public static string GetUploadedImagePath(string fileName)
        {
            var result = Path.Combine(ImageUploadsDir, fileName);
            return result;
        }

        public static void CleanupUploadedImageStorage()
        {
            try
            {
                var storage = GetUploadedImageStorage();
                if (storage.DirectoryExists(ImageUploadsDir))
                {
                    storage.DeleteDirectory(ImageUploadsDir);
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch {}
        }

        public static string GenerateUploadedFileName(IsolatedStorageFile storage, string fileExtension)
        {
            var result = default(string);

            while (result == null || storage.FileExists(GetUploadedImagePath(result)))
            {
                result = Guid.NewGuid() + "." + fileExtension;
            }

            return result;
        }

        public static string GetPictureUrl(string picture, IStorageProvider storageProvider)
        {
            if (picture == null) return null;
            if (picture.IsWebUrl()) return picture;

            var result = storageProvider.GetPublicUrl(picture);
            return result;
        }

        public static string SaveUploadedPicture(string previousPicture, string picture, 
            string storagePath, IStorageProvider storageProvider)
        {
            var result = picture;

            var previousPictureUrl = GetPictureUrl(previousPicture, storageProvider);
            if (previousPictureUrl != picture && picture != null) // if picture has uploaded file name
            {
                var fileName = Path.GetFileNameWithoutExtension(picture);
                Guid fileGuid;
                if (Guid.TryParse(fileName, out fileGuid)) // make sure file name is a valid Guid (not a malformed path)
                {
                    var uploadedStorage = GetUploadedImageStorage();
                    var uploadedFilePath = GetUploadedImagePath(picture);
                    using (var uploadedStream = uploadedStorage.OpenFile(uploadedFilePath, FileMode.Open))
                    {
                        var storageFilePath = Path.Combine(storagePath, picture);
                        storageProvider.SaveStream(storageFilePath, uploadedStream);
                        result = storageFilePath;

                        // if previous picture is in media storage then delete it
                        if (previousPicture != null && !previousPicture.IsWebUrl() &&
                            storageProvider.FileExists(previousPicture))
                        {
                            storageProvider.DeleteFile(previousPicture);
                        }
                    }

                    uploadedStorage.DeleteFile(uploadedFilePath);
                }
            }

            return result;
        }
    }
}