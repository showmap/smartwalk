using System;
using System.Web.Helpers;
using System.Web.Mvc;
using SmartWalk.Server.Controllers.Base;
using SmartWalk.Server.Utils;

namespace SmartWalk.Server.Controllers
{
    public class FileController : BaseController
    {
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

                return Json(new { fileName });
            }

            throw new InvalidOperationException("Ooopsi. There is no image to upload.");
        }
    }
}