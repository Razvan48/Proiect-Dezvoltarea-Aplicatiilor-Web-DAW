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

        [NonAction]
        private bool relevantDiscussion(Discussion dis, string search)
        {
            if (dis.Title.Trim().ToLower().Contains(search))
                return true;
            if (dis.Content.Trim().ToLower().Contains(search))
                return true;

            if (dis.Answers != null)
            {
                foreach (var ans in dis.Answers)
                {
                    if (ans.Content.Trim().ToLower().Contains(search))
                        return true;

                    if (ans.Comments != null)
                    {
                        foreach (var comm in ans.Comments)
                        {
                            if (comm.Content.Trim().ToLower().Contains(search))
                                return true;
                        }
                    }
                }
            }

            return false;
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

            var discussions = db.Discussions.Where(dis => dis.CategoryId == id);

            if (sortType == 1) // sortare dupa titlu
            {
                discussions = discussions.OrderBy(dis => dis.Title);
            }
            else if (sortType == 2) // sortare custom (dupa popularitate (numarul de answers + comments per discutie))
            {
                discussions = discussions.OrderByDescending(dis => dis.Answers.Count() + dis.Answers.SelectMany(ans => ans.Comments).Count());
            }
            else if (sortType == 3) // sortare dupa numarul de upvote-uri al discutiei
            {
                discussions = discussions.OrderByDescending(dis => db.Votes.Count(vote => vote.DiscussionId == dis.Id && vote.DidVote == 1) - db.Votes.Count(vote => vote.DiscussionId == dis.Id && vote.DidVote == 2));
            }

            var discussionsList = discussions.ToList();

            // search engine-ul
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                // eliminam spatiile libere din stanga de tot si dreapta de tot
                var search = Convert.ToString(HttpContext.Request.Query["search"]).Trim().ToLower();

                if (search != "")
                {
                    ViewBag.SearchString = search;

                    discussionsList.RemoveAll(dis => !relevantDiscussion(dis, search));
                }
            }

            //paginare
            int _perPage = 3;
            int totalItems = discussionsList.Count();
            var currentPage = page;
            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }
            var paginatedDiscussions = discussionsList.Skip(offset).Take(_perPage);
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

