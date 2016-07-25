using System.Web.Mvc;

namespace Pentathanerd.When.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        { 
            return View();
        }

        public ActionResult HardReset(string resetKey)
        {
            if (resetKey == "hardToGuessResetKey123")
                GamePlayHub.HardReset();

            return RedirectToAction("Index");
        }
    }
}