using System.Web.Mvc;

namespace DotNet5CloudWeb.Controllers {
    public class AlbumController : Controller {
        public ActionResult PopulateDb() {
            return View();
        }
    }
}