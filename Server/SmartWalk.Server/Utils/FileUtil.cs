using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using Orchard.FileSystems.Media;
using Orchard.Logging;
using Orchard.MediaProcessing.Services;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Utils
{
    public static class FileUtil
    {
        private const string ImageUploadsDir = "ImageUploads";

        public static IsolatedStorageFile GetUploadedImageStorage()
        {
            var storage = IsolatedStorageFile.GetMachineStoreForAssembly();
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
                result = GenerateFileName(fileExtension);
            }

            return result;
        }

        public static string GenerateFileName(string fileExtension)
        {
            var result = Guid.NewGuid() + "." + fileExtension;
            return result;
        }

        public static string GetPictureUrl(string picture, IStorageProvider storageProvider)
        {
            if (picture == null) return null;
            if (picture.IsWebUrl()) return picture;

            var result = storageProvider.GetPublicUrl(picture);
            return result;
        }

        /// <summary>
        /// Handles routine around uploaded pictures. Moves uploaded pictures into Azure storage. Deletes previous pictures.
        /// </summary>
        public static string ProcessUploadedPicture(string previousPicture, string picture, 
            string storagePath, IStorageProvider storageProvider)
        {
            var result = previousPicture;

            if (picture != null)
            {
                // if picture has a valid uploaded file name
                if (IsFileNameValid(picture))
                {
                    var uploadedStorage = GetUploadedImageStorage();
                    var uploadedFilePath = GetUploadedImagePath(picture);
                    if (uploadedStorage.FileExists(uploadedFilePath))
                    {
                        using (var uploadedStream = uploadedStorage.OpenFile(uploadedFilePath, 
                            FileMode.Open, FileAccess.Read))
                        {
                            var storageFilePath = Path.Combine(storagePath, picture);
                            storageProvider.SaveStream(storageFilePath, uploadedStream);

                            result = storageFilePath;
                            DeleteFile(previousPicture, storageProvider);
                        }

                        try
                        {
                            // trying delete, it may be being read by image downloading handler.
                            uploadedStorage.DeleteFile(uploadedFilePath);
                        }
                        // ReSharper disable once EmptyGeneralCatchClause
                        catch {}
                    }
                }
            }
            else
            {
                result = null;
                DeleteFile(previousPicture, storageProvider);
            }

            // NOTE: it's not allowed for now to accept external Urls for picture field

            return result;
        }

        /// <summary>
        /// Gets a value indicating whether file name is a valid Guid and not a malformed path.
        /// </summary>
        public static bool IsFileNameValid(string fileName)
        {
            Guid fileGuid;
            return Guid.TryParse(Path.GetFileNameWithoutExtension(fileName), out fileGuid);
        }

        public static void ResizePictures(IEnumerable<IPicture> pictures, PictureSize? size,
            IImageProfileManager imageProfileManager, ILogger logger = null)
        {
            if (pictures == null || size == null || size == PictureSize.Full) return;

            foreach (var picture in pictures)
            {
                ResizePicture(picture, size, imageProfileManager, logger);
            }
        }

        public static void ResizePicture(IPicture pictureModel, PictureSize? size, 
            IImageProfileManager imageProfileManager, ILogger logger = null)
        {
            if (pictureModel == null || size == null || size == PictureSize.Full) return;

            pictureModel.Picture = GetResizedPicture(pictureModel.Picture, size, imageProfileManager, logger);
        }

        public static string GetResizedPicture(string picture, PictureSize? size,
            IImageProfileManager imageProfileManager, ILogger logger = null)
        {
            if (size == null || size == PictureSize.Full) return picture;

            var result = picture;

            try
            {
                result = imageProfileManager.GetImageProfileUrl(picture, size.ToString());
            }
            catch (Exception e)
            {
                if (logger != null)
                {
                    logger.Debug("Exception '{0}' on attempt to build a thumbnail for '{1}'.", e.Message, picture);
                }
            }

            return result;
        }

        public static Pictures GetResizedPictures(string picture, 
            IImageProfileManager imageProfileManager, ILogger logger = null)
        {
            var result = default(Pictures);

            if (picture != null)
            {
                result = new Pictures
                {
                    Small = GetResizedPicture(picture, PictureSize.Small, imageProfileManager, logger),
                    Medium = GetResizedPicture(picture, PictureSize.Medium, imageProfileManager, logger),
                    Full = picture
                };
            }

            return result;
        }

        private static void DeleteFile(string fileName, IStorageProvider storageProvider)
        {
            // if previous picture is in media storage then delete it
            if (fileName != null && !fileName.IsWebUrl() && storageProvider.FileExists(fileName))
            {
                storageProvider.DeleteFile(fileName);
            }
        }
    }
}