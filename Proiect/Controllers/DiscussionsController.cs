using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proiect.Data;
using Proiect.Models;

namespace Proiect.Controllers
{
    public class DiscussionsController : Controller
    {
        public readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public DiscussionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Edit(int id)
        {
            Discussion discussion = db.Discussions.Where(dis => dis.Id == id).First();

            return View(discussion);
        }

        [HttpPost]
        public IActionResult Edit(int id, Discussion requestDiscussion)
        {
            Discussion discussion = db.Discussions.Find(id);

            requestDiscussion.Date = DateTime.Now;

            if (ModelState.IsValid)
            {
                discussion.Title = requestDiscussion.Title;
                discussion.Content = requestDiscussion.Content;
                db.SaveChanges();
                TempData["message"] = "Discussion successfully edited";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestDiscussion);
            }
        }

        // Nu stiu daca e cel mai frumos mod, dar new-ul de mai jos primeste ca parametru si id-ul
        // categoriei unde va fi adaugat.
        public IActionResult New(int categoryId)
        {
            Discussion discussion = new Discussion();

            ViewBag.CategoryId = categoryId;

            return View(discussion);
        }

        [HttpPost]
        public IActionResult New(Discussion discussion)
        {
            discussion.Date = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Discussions.Add(discussion);
                db.SaveChanges();

                TempData["message"] = "Discussion successfully added";

                return RedirectToAction("/Categories/Index/" + discussion.CategoryId);
            }
            else
            {
                return View(discussion);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Discussion discussion = db.Discussions.Find(id);

            int? categoryId = discussion.CategoryId;

            db.Discussions.Remove(discussion);
            db.SaveChanges();

            TempData["message"] = "Discussion successfully deleted";

            return RedirectToAction("/Categories/Show/" + categoryId);
        }
    }
}

