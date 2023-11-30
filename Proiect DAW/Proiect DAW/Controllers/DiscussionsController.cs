using Microsoft.AspNetCore.Mvc;

namespace Proiect_DAW.Controllers
{
    public class DiscussionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
