using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // oricine are dreptul sa vada
        // Afisare discutie impreuna cu toate raspunsurile + comentariile
        [HttpGet]
        public IActionResult Show(int id)
        {
            Discussion discussion = db.Discussions.Include("User").Include("Answers").Include("Answers.User").Include("Answers.Comments").Include("Answers.Comments.User")
                                    .Where(dis => dis.Id == id)
                                    .First();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
            }

            // ignora toate campurile
            ViewBag.CheckAnswer = false;
            ViewBag.CheckComment = -1;

            SetAccessRights();

            return View(discussion);
        }

        // Postare raspuns
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult AddAnswer([FromForm] Answer answer)
        {
            // TODO: trebuie sa avem o functie SHOW pentru toate cele 3 metode
            // sau la sfarsit facem RedirectToAction

            answer.Date = DateTime.Now;
            answer.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Answers.Add(answer);
                db.SaveChanges();

                TempData["message"] = "Raspunsul a fost postat";
                TempData["messageType"] = "alert-success";

                return Redirect("/Discussions/Show/" + answer.DiscussionId);
            }
            else
            {
                Discussion discussion = db.Discussions.Include("User").Include("Answers").Include("Answers.User").Include("Answers.Comments").Include("Answers.Comments.User")
                                        .Where(dis => dis.Id == answer.DiscussionId)
                                        .First();

                // ignora campurile de comentarii
                ViewBag.CheckAnswer = true;
                ViewBag.CheckComment = -1;

                SetAccessRights();

                return View("Show", discussion);
            }
        }

        // Postare comentariu
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult AddComment([FromForm] Comment comment)
        {
            // TODO: trebuie sa avem o functie SHOW pentru toate cele 3 metode
            // sau la sfarsit facem RedirectToAction

            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();

                TempData["message"] = "Comentariul a fost postat";
                TempData["messageType"] = "alert-success";

                Answer answer = db.Answers.Find(comment.AnswerId);

                return Redirect("/Discussions/Show/" + answer.DiscussionId);
            }
            else
            {
                Answer answer = db.Answers.Find(comment.AnswerId);

                Discussion discussion = db.Discussions.Include("User").Include("Answers").Include("Answers.User").Include("Answers.Comments").Include("Answers.Comments.User")
                        .Where(dis => dis.Id == answer.DiscussionId)
                        .First();

                // TODO: am un bug aici
                // ignora campul de raspuns
                ViewBag.CheckAnswer = false;
                ViewBag.CheckComment = comment.AnswerId;

                SetAccessRights();

                return View("Show", discussion);
            }
        }

        // Editare discutie
        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult Edit(int id, int categoryId)
        {
            Discussion discussion = db.Discussions
                                    .Where(dis => dis.Id == id)
                                    .First();

            // TODO: delete? nu pare folosit in view
            ViewBag.CategoryId = categoryId;

            // verificam daca discutia ii apartine user-ului care incearca sa editeze /SAU/ daca este admin
            if (discussion.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(discussion);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei discutii care nu va apartine";
                TempData["messageType"] = "alert-danger";

                return Redirect("/Discussions/Show/" + discussion.Id);
            }
        }

        // Se adauga discutia editata in baza de date
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Edit(int id, Discussion requestDiscussion)
        {
            Discussion discussion = db.Discussions.Find(id);

            requestDiscussion.Date = DateTime.Now;

            if (ModelState.IsValid)
            {
                // verificam daca discutia ii apartine user-ului care incearca sa editeze /SAU/ daca este admin
                if (discussion.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    discussion.Title = requestDiscussion.Title;
                    discussion.Content = requestDiscussion.Content;
                    db.SaveChanges();

                    TempData["message"] = "Discussion successfully edited";
                    TempData["messageType"] = "alert-success";

                    return RedirectToAction("Show", "Categories", new { Id = discussion.CategoryId });
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei discutii care nu va apartine";
                    TempData["messageType"] = "alert-danger";

                    return RedirectToAction("Show", "Categories", new { Id = discussion.CategoryId });
                }
            }
            else
            {
                return View(requestDiscussion);
            }
        }

        // TODO: Nu stiu daca e cel mai frumos mod, dar new-ul de mai jos primeste ca parametru si id-ul categoriei unde va fi adaugat
        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult New(int categoryId)
        {
            Discussion discussion = new Discussion();

            // TODO: delete? nu pare folosit in view
            ViewBag.CategoryId = categoryId;

            return View(discussion);
        }

        // Adauga noua discutie in baza de date
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New(Discussion discussion)
        {
            discussion.Date = DateTime.Now;
            discussion.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Discussions.Add(discussion);
                db.SaveChanges();

                TempData["message"] = "Discussion successfully added";
                TempData["messageType"] = "alert-success";

                return RedirectToAction("Show", "Categories", new { Id = discussion.CategoryId });
            }
            else
            {
                return View(discussion);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // TODO: ERROR: nu sterge si Answers automat

            Discussion discussion = db.Discussions.Include("Answers").Include("Category")
                                    .Where(dis => dis.Id == id)
                                    .First();

            // verificam daca discutia ii apartine user-ului care incearca sa editeze /SAU/ daca este admin
            if (discussion.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Discussions.Remove(discussion);
                db.SaveChanges();

                TempData["message"] = "Discussion successfully deleted";
                TempData["messageType"] = "alert-success";

                return RedirectToAction("Show", "Categories", new { Id = discussion.CategoryId });
            }
            else
            {
                TempData["message"] = "Discussion successfully deleted";
                TempData["messageType"] = "alert-danger";

                return RedirectToAction("Show", "Categories", new { Id = discussion.CategoryId });
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

