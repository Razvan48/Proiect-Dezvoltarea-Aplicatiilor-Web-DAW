using Microsoft.AspNetCore.Mvc;

namespace Proiect_DAW.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
