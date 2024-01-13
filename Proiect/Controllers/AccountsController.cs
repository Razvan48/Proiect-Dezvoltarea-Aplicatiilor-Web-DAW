using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using Proiect.Data;
using Proiect.Models;
using System.Data;
using System.Net;

namespace Proiect.Controllers
{
    public class AccountsController : Controller
    {
        public readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IWebHostEnvironment _env;

        public AccountsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment env)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult UnCoolDown(string id)
        {
            var user = db.ApplicationUsers.Find(id);

            if (user != null)
            {
                user.CoolDownEnd = DateTime.MinValue;
                db.SaveChanges();

                TempData["message"] = "User uncooled";
                TempData["messageType"] = "alert-info";
            }
            else
            {
                TempData["message"] = "User id does not exist";
                TempData["messageType"] = "alert-info";
            }

            return RedirectToAction("Index");
        }



        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult SetCoolDown(string id, DateTime coolDownEnd)
        {
            if (coolDownEnd < DateTime.Now)
            {
                TempData["message"] = "Invalid cooldown date";
                TempData["messageType"] = "alert-info";

                return RedirectToAction("Index");
            }

            var user = db.ApplicationUsers.Find(id);

            if (user != null)
            {
                user.CoolDownEnd = coolDownEnd;
                db.SaveChanges();

                TempData["message"] = "Cooldown successfully set";
                TempData["messageType"] = "alert-info";
            }
            else
            {
                TempData["message"] = "User id does not exist";
                TempData["messageType"] = "alert-info";
            }

            return RedirectToAction("Index");
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Index()
        {
            var users = db.Users.ToList();
            ViewBag.Users = users;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult PromoteToAdmin(string id)
        {
            ApplicationUser user = _userManager.FindByIdAsync(id).Result;

            if (user != null)
            {
                if (!_userManager.IsInRoleAsync(user, "Admin").Result)
                {
                    var rezultat = _userManager.AddToRoleAsync(user, "Admin").Result;
                }
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult DemoteToUser(string id)
        {
            ApplicationUser user = _userManager.FindByIdAsync(id).Result;

            if (user != null)
            {
                if (_userManager.IsInRoleAsync(user, "Admin").Result)
                {
                    var rezultat = _userManager.RemoveFromRoleAsync(user, "Admin").Result;
                }
            }

            return RedirectToAction("Index");
        }


        // oricine poate sa vada profilul unui utilizator
        [HttpGet]
        public async Task<ActionResult> Show(string id)
        {
            ApplicationUser user = db.Users.Include("Discussions").Include("Answers").Include("Comments")
                                   .Where(u =>  u.Id == id)
                                   .First();

            SetAccessRights();
            

            // number of votes the user has
            var userTotal = 0;
            foreach (var discussion in user.Discussions) {
                userTotal += db.Votes.Count(vote => vote.DiscussionId == discussion.Id && vote.DidVote == 1) - db.Votes.Count(vote => vote.DiscussionId == discussion.Id && vote.DidVote == 2);
            }
            foreach (var answer in user.Answers) {
                userTotal += db.Votes.Count(vote => vote.AnswerId == answer.Id && vote.DidVote == 1) - db.Votes.Count(vote => vote.AnswerId == answer.Id && vote.DidVote == 2);
            }
            ViewBag.votesTotal = userTotal;

            //number of awards
            var userAwards = 0;

            foreach (var answer in user.Answers) {
                if (answer.hasAward == true)
                    ++userAwards;
            }
            ViewBag.awardsTotal = userAwards;

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            ViewBag.Role = role;

            if (DateTime.Now <= user.CoolDownEnd)
            {
                var timeDiff = user.CoolDownEnd - DateTime.Now;
                ViewBag.Days = timeDiff.Days;
                timeDiff -= TimeSpan.FromDays(timeDiff.Days);
                ViewBag.Hours = timeDiff.Hours;
                timeDiff -= TimeSpan.FromHours(timeDiff.Hours);
                ViewBag.Minutes = timeDiff.Minutes;
                timeDiff -= TimeSpan.FromMinutes(timeDiff.Minutes);
                ViewBag.Seconds = timeDiff.Seconds;
            }

            return View(user);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);

            // verifica daca este profilul utilizatorului curent
            if (user.Id == _userManager.GetUserId(User))
            {
                return View(user);
            }
            else
            {
                return Redirect("/Home");
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, ApplicationUser requestedUser)
        {
            var sanitizer = new HtmlSanitizer();

            ApplicationUser user = db.Users.Find(id);

            // verifica daca este profilul utilizatorului curent
            if (user.Id == _userManager.GetUserId(User))
            {
                if (ModelState.IsValid)
                {
                    user.FirstName = requestedUser.FirstName;
                    user.LastName = requestedUser.LastName;
                    requestedUser.AboutMe = sanitizer.Sanitize(requestedUser.AboutMe);
                    user.AboutMe = requestedUser.AboutMe;

                    db.SaveChanges();
                
                    return Redirect("/Accounts/Show/" + id);
                }
                else
                {
                    return View(requestedUser);
                }
            }
            else
            {
                return Redirect("/Home");
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> UploadImage(string id, IFormFile UserImage)
        {
            ApplicationUser user = db.Users.Find(id);

            // verifica daca este profilul utilizatorului curent
            if (user.Id == _userManager.GetUserId(User))
            {
                // salveaza poza
                if (UserImage != null && UserImage.Length > 0)
                {
                    var storagePath = Path.Combine(_env.WebRootPath, "images", UserImage.FileName);
                    var databaseFileName = "/images/" + UserImage.FileName;

                    using (var fileStream = new FileStream(storagePath, FileMode.Create))
                    {
                        await UserImage.CopyToAsync(fileStream);
                    }

                    user.Image = databaseFileName;
                    db.SaveChanges();
                }
            }

            return Redirect("/Accounts/Show/" + id);
        }

        // Conditii de afisare a butoanelor de editare si stergere
        [NonAction]
        private void SetAccessRights()
        {
            ViewBag.IsAdmin = User.IsInRole("Admin");
            ViewBag.CurrentUser = _userManager.GetUserId(User);
        }
    }
}

