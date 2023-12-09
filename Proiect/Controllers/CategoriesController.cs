using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Models;
using System.Data;

namespace Proiect.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;

        public CategoriesController(ApplicationDbContext context)
        {
            db = context;
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

            return View(category);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Category category = db.Categories.Where(cat => cat.Id == id).First();

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(int id, Category requestCategory)
        {
            Category category = db.Categories.Find(id);

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

        [HttpGet]
        public IActionResult New()
        {
            Category category = new Category();

            return View(category);
        }

        [HttpPost]
        public IActionResult New(Category category)
        {
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

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);

            db.Categories.Remove(category);
            db.SaveChanges();

            TempData["message"] = "Discussion Category successfully deleted";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Index");
        }
    }
}

