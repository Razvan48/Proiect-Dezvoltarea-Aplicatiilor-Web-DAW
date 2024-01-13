using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proiect.Models;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Proiect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;

            _signInManager = signInManager;

            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var user = User.Identity;

            if (user != null && user.IsAuthenticated)
            {
                ApplicationUser applicationUser = _userManager.GetUserAsync(User).Result;

                if (applicationUser.CoolDownEnd >= DateTime.Now)
                {
                    var result =  _signInManager.SignOutAsync();

                    var timeDiff = applicationUser.CoolDownEnd - DateTime.Now;
                    var days = timeDiff.Days;
                    timeDiff -= TimeSpan.FromDays(days);
                    var hours = timeDiff.Hours;
                    timeDiff -= TimeSpan.FromHours(hours);
                    var minutes = timeDiff.Minutes;
                    timeDiff -= TimeSpan.FromMinutes(minutes);
                    var seconds = timeDiff.Seconds;

                    ViewBag.Message = "Nu va puteti loga cu acest cont (contul se afla in cooldown pentru urmatoarele " + days + " zile, " + hours + " ore, " + minutes + " minute, " + seconds + " secunde";
                    ViewBag.Alert = "alert-info";
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}