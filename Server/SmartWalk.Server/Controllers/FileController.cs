using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using SmartWalk.Server.Controllers.Base;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Controllers
{
    public class FileController : BaseController
    {
        private const int MaxUploadSizeMB = 15;
        private const int MaxUploadSize = MaxUploadSizeMB * 1024 * 1024;
        private const int MaxDimension = 4000;

        [Authorize]
        public ActionResult UploadedImagePreview(string fileName)
        {
            if (FileUtil.IsFileNameValid(fileName))
            {
                var storage = FileUtil.GetUploadedImageStorage();
                if (storage.FileExists(FileUtil.GetUploadedImagePath(fileName)))
                {
                    var stream = storage.OpenFile(FileUtil.GetUploadedImagePath(fileName), FileMode.Open);
                    return File(stream, MimeMapping.GetMimeMapping(fileName));
                }
            }

            HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return new EmptyResult();
        }

        [Authorize]
        [HttpPost]
        public ActionResult UploadImage()
        {
            var webImage = WebImage.GetImageFromRequest();
            if (webImage != null)
            {
                if (webImage.GetBytes().Length > MaxUploadSize)
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new ErrorResultVm(new[]
                        {
                            new ValidationError
                                {
                                    Property = T("Size").Text,
                                    Error = T("The image size is bigger than {0} MB.", MaxUploadSizeMB).Text
                                }
                        }));
                }

                ResizeImage(webImage);

                var storage = FileUtil.GetUploadedImageStorage();
                var fileName = FileUtil.GenerateUploadedFileName(storage, webImage.ImageFormat);
                using (var stream = storage.CreateFile(FileUtil.GetUploadedImagePath(fileName)))
                {
                    var bytes = webImage.GetBytes();
                    stream.Write(bytes, 0, bytes.Length);
                }

                return Json(new { fileName, url = Url.Action("UploadedImagePreview", new { fileName }) });
                
            }

            throw new InvalidOperationException("Ooopsi. There is no image to upload.");
        }

        public static void ResizeImage(WebImage image)
        {
            if (image.Width > MaxDimension || image.Height > MaxDimension)
            {
                image.Resize(MaxDimension, MaxDimension, true, true);
            }
        }
    }
}