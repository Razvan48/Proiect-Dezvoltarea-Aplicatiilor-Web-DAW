using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Models;

namespace Proiect.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;

        public CategoriesController(ApplicationDbContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            var categories = db.Categories;

            ViewBag.Categories = categories;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
            }

            return View();
        }

        public IActionResult Show(int id)
        {
            Category category = db.Categories.Include("Discussions").Where(cat => cat.Id == id).First();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
            }

            return View(category);
        }

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
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestCategory);
            }
        }

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

            return RedirectToAction("Index");
        }
    }
}

