using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using SmartWalk.Server.Controllers.Base;
using SmartWalk.Server.Utils;

namespace SmartWalk.Server.Controllers
{
    public class FileController : BaseController
    {
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
                var bytes = webImage.GetBytes();
                if (bytes.Length > 2000000) throw new InvalidOperationException("Too big");

                // TODO: To add validation of image dimensions

                var storage = FileUtil.GetUploadedImageStorage();
                var fileName = FileUtil.GenerateUploadedFileName(storage, webImage.ImageFormat);
                using (var stream = storage.CreateFile(FileUtil.GetUploadedImagePath(fileName)))
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                return Json(new { fileName, url = Url.Action("UploadedImagePreview", new { fileName }) });
                
            }

            throw new InvalidOperationException("Ooopsi. There is no image to upload.");
        }
    }
}