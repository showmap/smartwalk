using System.IO;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Orchard.Themes;
using SmartWalk.Server.Extensions;

namespace SmartWalk.Server.Controllers
{
    [HandleError, Themed]
    public class StaticController : Controller
    {
        private readonly static string ModuleName = Assembly.GetExecutingAssembly().GetName().Name;
        private readonly static string ModulePath = HostingEnvironment.MapPath(@"~/Modules/" + ModuleName);
        private readonly static string RootPath = Path.Combine(ModulePath, "Views/Static/Root/"); 

        [CompressFilter]
        public ActionResult Root(string folderName = null, string subFolderName = null, string fileName = null)
        {
            var file = default(string);
            folderName = StripSlashes(folderName);
            subFolderName = StripSlashes(subFolderName);
            fileName = StripSlashes(fileName);

            if (folderName == null || fileName == null)
            {
                file = Path.Combine(RootPath, "index.html");
            }
            else
            {
                file = Path.Combine(RootPath, folderName, subFolderName ?? string.Empty, fileName);
            }

            if (System.IO.File.Exists(file))
            {
                return File(file, MimeMapping.GetMimeMapping(file));
            }
            else
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return new EmptyResult();
            }
        }

        private string StripSlashes(string path)
        {
            if (path != null)
            {
                path = path.Replace(@"\", string.Empty);
                path = path.Replace(@"/", string.Empty);
            }
            return path;
        }
    }
}