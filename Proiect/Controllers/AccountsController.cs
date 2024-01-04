using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using Proiect.Data;
using Proiect.Models;
using System.Data;

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

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            ViewBag.Role = role;

            return View(user);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);

            return View(user);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, ApplicationUser requestedUser, IFormFile UserImage)
        {
            ApplicationUser user = db.Users.Find(id);

            // TODO: trb sa punem o poza default pt orice utilizator

            if (ModelState.IsValid)
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
                }

                user.FirstName = requestedUser.FirstName;
                user.LastName = requestedUser.LastName;

                db.SaveChanges();
                
                return Redirect("/Accounts/Show/" + id);
            }
            else
            {
                return View(requestedUser);
            }
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

