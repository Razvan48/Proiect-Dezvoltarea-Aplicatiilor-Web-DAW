using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Models;
using System.Data;
using System.Globalization;

namespace Proiect.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Sterge din baza de date un raspuns postat
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Comment comment = db.Comments.Include("Answer")
                              .Where(c => c.Id == id)
                              .First();

            // sterge notificarile care aveau legatura cu aceast comentariu
            List<Notification> notifications = db.Notifications.Include("User")
                                               .Where(not => not.CommentId == comment.Id)
                                               .ToList();

            foreach (Notification notification in notifications)
            {
                if (notification.Read == false)
                {
                    notification.User.UnreadNotifications--;
                }

                db.Notifications.Remove(notification);
            }

            db.SaveChanges();

            // verificam daca discutia ii apartine user-ului care incearca sa editeze /SAU/ daca este admin
            if (comment.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comment);
                db.SaveChanges();

                TempData["message"] = "Comentariul a fost sters";
                TempData["messageType"] = "alert-success";
            }
            else
            {
                TempData["message"] = "Nu puteti sa stergeti comentariul altor utilizatori";
                TempData["messageType"] = "alert-danger";
            }

            return Redirect("/Discussions/Show/" + comment.Answer.DiscussionId);
        }

        [Authorize(Roles = "User,Editor,Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Comment comment = db.Comments.Include("Answer")
                              .Where(c => c.Id == id)
                              .First();

            if (comment.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                TempData["EditCommentID"] = id;
                return Redirect("/Discussions/Show/" + comment.Answer.DiscussionId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati un comentariu care nu va apartine"; ;
                TempData["messageType"] = "alert-success";

                return Redirect("/Discussions/Show/" + comment.Answer.DiscussionId);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comment = db.Comments.Include("Answer").Include("Answer.Discussion").Include("Answer.User")
                              .Where(c => c.Id == id)
                              .First();

            requestComment.Date = DateTime.Now;

            if (comment.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    comment.Content = requestComment.Content;
                    comment.Date = requestComment.Date;
                    db.SaveChanges();

                    // adauga notificare daca nu comentezi la propriul raspuns
                    if (comment.Answer.UserId != comment.UserId)
                    {
                        Notification NewNotification = new Notification
                        {
                            Read = false,
                            DateMonth = DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture),
                            DateDay = DateTime.Now.Day,
                            UserId = comment.Answer.UserId,
                            DiscussionId = comment.Answer.DiscussionId,
                            AnswerId = comment.Answer.Id,
                            CommentId = comment.Id,
                            Type = 7
                        };

                        // incrementeaza nr de notificari necitite ale user-ului
                        comment.Answer.User.UnreadNotifications++;

                        db.Notifications.Add(NewNotification);
                        db.SaveChanges();
                    }

                    TempData["message"] = "Comentariul a fost editat";
                    TempData["messageType"] = "alert-success";

                    return Redirect("/Discussions/Show/" + comment.Answer.DiscussionId);
                }
                else
                {
                    TempData["message"] = "Continutul raspunsului nu poate fi null"; ;
                    TempData["messageType"] = "alert-danger";

                    return Redirect("/Comments/Edit/" + comment.Id);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati un comentariu care nu va apartine";
                TempData["messageType"] = "alert-danger";

                return Redirect("/Discussions/Show/" + comment.Answer.DiscussionId);
            }
        }
    }
}

