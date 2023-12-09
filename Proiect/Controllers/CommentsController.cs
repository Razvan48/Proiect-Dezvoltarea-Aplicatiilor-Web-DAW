using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Models;
using System.Data;

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

            // verificam daca discutia ii apartine user-ului care incearca sa editeze /SAU/ daca este admin
            if (comment.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comment);
                db.SaveChanges();

                TempData["message"] = "Comment successfully deleted";
                TempData["messageType"] = "alert-success";
            }
            else
            {
                TempData["message"] = "Comment successfully deleted";
                TempData["messageType"] = "alert-danger";
            }

            return Redirect("/Discussions/Show/" + comment.Answer.DiscussionId);
        }

        // TODO: editare directa din Discussions/Show/Id?
        // TODO: Edit
        // TODO: POST Edit
    }
}

