using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Models;
using System.Data;

namespace Proiect.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public CategoriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var categories = db.Categories;

            ViewBag.Categories = categories;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
            }

            SetAccessRights();

            return View();
        }

        [HttpGet]
        public IActionResult Show(int id)
        {
            Category category = db.Categories.Include("Discussions").Where(cat => cat.Id == id).First();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
            }

            SetAccessRights();

            return View(category);
        }

        [Authorize(Roles = "Editor,Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Category category = db.Categories.Where(cat => cat.Id == id).First();

            SetAccessRights();

            return View(category);
        }

        [Authorize(Roles = "Editor,Admin")]
        [HttpPost]
        public IActionResult Edit(int id, Category requestCategory)
        {
            Category category = db.Categories.Find(id);

            SetAccessRights();

            if (ModelState.IsValid)
            {
                category.CategoryName = requestCategory.CategoryName;
                db.SaveChanges();

                TempData["message"] = "Discussion Category successfully edited";
                TempData["messageType"] = "alert-success";

                return RedirectToAction("Index");
            }
            else
            {
                return View(requestCategory);
            }
        }

        [Authorize(Roles = "Editor,Admin")]
        [HttpGet]
        public IActionResult New()
        {
            Category category = new Category();

            SetAccessRights();

            return View(category);
        }

        [Authorize(Roles = "Editor,Admin")]
        [HttpPost]
        public IActionResult New(Category category)
        {
            SetAccessRights();

            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();

                TempData["message"] = "Discussion Category successfully added";
                TempData["messageType"] = "alert-success";

                return RedirectToAction("Index");
            }
            else
            {
                return View(category);
            }
        }

        [Authorize(Roles = "Editor,Admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);

            db.Categories.Remove(category);
            db.SaveChanges();

            TempData["message"] = "Discussion Category successfully deleted";
            TempData["messageType"] = "alert-success";

            SetAccessRights();

            return RedirectToAction("Index");
        }


        // Conditii de afisare a butoanelor de editare si stergere
        [NonAction]
        private void SetAccessRights()
        {
            ViewBag.IsAdmin = User.IsInRole("Admin");
            ViewBag.IsEditor = User.IsInRole("Editor");
            ViewBag.CurrentUser = _userManager.GetUserId(User);
        }
    }
}

