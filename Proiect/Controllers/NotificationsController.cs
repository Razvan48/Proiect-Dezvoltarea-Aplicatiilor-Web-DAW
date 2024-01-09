using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Data.Migrations;
using Proiect.Models;
using System.Data;

namespace Proiect.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public NotificationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult Index(string id)
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
            }

            // utilizatorul are acces doar la inbox-ul propriu
            string CurrentUserId = _userManager.GetUserId(User);

            if (id == null || CurrentUserId != id)
            {
                return Redirect("/Home/Index");
            }

            ViewBag.Notifications = db.Notifications.Include("Discussion").Include("Answer").Include("Comment").Include("Answer.User").Include("Comment.User")
                                    .Where(n => n.UserId == CurrentUserId)
                                    .OrderByDescending(n => n.Id)
                                    .ToList();

            return View();
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult MarkAsRead(int id)
        {
            Notification notification = db.Notifications.Include("Discussion").Include("User")
                                        .Where(n => n.Id == id)
                                        .First();

            if (notification.Read == false)
            {
                notification.Read = true;
                notification.User.UnreadNotifications--;
            }
            db.SaveChanges();

            return Redirect("/Discussions/Show/" + notification.DiscussionId);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Notification notification = db.Notifications.Find(id);

            // verificam daca notificarea ii apartine user-ului care incearca sa il stearga
            if (notification.UserId == _userManager.GetUserId(User))
            {
                db.Notifications.Remove(notification);
                db.SaveChanges();

                TempData["message"] = "Notificarea a fost stearsa";
                TempData["messageType"] = "alert-success";
            }
            else
            {
                TempData["message"] = "Nu puteti sa stergeti notificarile altor utilizatori";
                TempData["messageType"] = "alert-danger";
            }

            return Redirect("/Notifications/Index/" + notification.UserId);
        }
    }
}

