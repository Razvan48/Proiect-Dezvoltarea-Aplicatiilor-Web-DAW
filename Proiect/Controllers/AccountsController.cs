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

