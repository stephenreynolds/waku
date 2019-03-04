using Microsoft.AspNetCore.Mvc;

namespace Waku.Controllers
{
    public class AppController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
