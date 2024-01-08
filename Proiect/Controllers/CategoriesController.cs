using Humanizer;
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
        public IActionResult Show(int id, int page = 1, int sortType = 0)
        {
            Category category = db.Categories.Include("Discussions").Where(cat => cat.Id == id).First();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
            }

            SetAccessRights();

            //paginare
            var discussions = db.Discussions.Where(dis => dis.CategoryId == id);
            //var sortType = Convert.ToInt32(HttpContext.Request.Query["sortType"]); (eroare)
            if (sortType == 1) // sortare dupa titlu
            {
                //discussions = db.Discussions.Where(dis => dis.CategoryId == id).OrderBy(dis => dis.Title);
                discussions = discussions.OrderBy(dis => dis.Title);
            }
            else if (sortType == 2) // sortare custom (dupa popularitate (numarul de answers + comments per discutie))
            {
                // discussions = discussions.OrderByDescending(dis => dis.Answers.Count());

                discussions = discussions.OrderByDescending(dis => dis.Answers.Count() + dis.Answers.SelectMany(ans => ans.Comments).Count());
            }
            int _perPage = 3;
            int totalItems = discussions.Count();
            //var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]); (eroare)
            var currentPage = page;
            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }
            var paginatedDiscussions = discussions.Skip(offset).Take(_perPage);
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            ViewBag.Discussions = paginatedDiscussions;

            return View(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Category category = db.Categories.Where(cat => cat.Id == id).First();

            SetAccessRights();

            return View(category);
        }

        [Authorize(Roles = "Admin")]
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

                return RedirectToAction("Show", "Categories", new { category.Id });
            }
            else
            {
                return View(requestCategory);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult New()
        {
            Category category = new Category();

            SetAccessRights();

            return View(category);
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Include("Discussions").Include("Discussions.Answers").Include("Discussions.Answers.Comments")
                      .Where(cat => cat.Id == id)
                      .First();

            // sterge manual discutiile + notificarile + raspunsurile + comentariile din aceasta categorie
            foreach (Discussion discussion in category.Discussions)
            {
                foreach (Answer answer in discussion.Answers)
                {
                    foreach (Comment comment in answer.Comments)
                    {
                        db.Comments.Remove(comment);
                    }

                    db.Answers.Remove(answer);
                }

                // sterge notificarile care aveau legatura cu aceasta discutie
                List<Notification> notifications = db.Notifications
                                                   .Where(not => not.DiscussionId == discussion.Id)
                                                   .ToList();

                foreach (Notification notification in notifications)
                {
                    db.Notifications.Remove(notification);
                }

                db.Discussions.Remove(discussion); 
            }
            
            db.SaveChanges();

            // sterge categoria
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
            ViewBag.CurrentUser = _userManager.GetUserId(User);
        }
    }
}

