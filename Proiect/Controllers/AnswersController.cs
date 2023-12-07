using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proiect.Data;
using Proiect.Data.Migrations;
using Proiect.Models;

namespace Proiect.Controllers
{
    public class AnswersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public AnswersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
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
            Answer answer = db.Answers.Find(id);

            // verificam daca discutia ii apartine user-ului care incearca sa editeze /SAU/ daca este admin
            if (answer.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Answers.Remove(answer);
                db.SaveChanges();

                TempData["message"] = "Answer successfully deleted";
                TempData["messageType"] = "alert-success";
            }
            else
            {
                TempData["message"] = "Answer successfully deleted";
                TempData["messageType"] = "alert-danger";
            }

            return Redirect("/Discussions/Show/" + answer.DiscussionId);
        }

        // TODO: editare directa din Discussions/Show/Id?
        // TODO: Edit
        // TODO: POST Edit
    }
}

