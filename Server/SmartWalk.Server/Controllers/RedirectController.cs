using System.Web.Mvc;

namespace SmartWalk.Server.Controllers
{
    public class RedirectController : Controller
    {
        public ActionResult Index(string host, string path)
        {
            return RedirectPermanent(host + path);
        }
    }
}