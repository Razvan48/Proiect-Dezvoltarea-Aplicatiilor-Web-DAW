using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Data.Migrations;
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
            Discussion discussion = db.Discussions
                                        .Include("User")
                                        .Include(d => d.Votes)  // Include discussion votes
                                        .Include(d => d.Answers)
                                            .ThenInclude(a => a.User)  // Include answer user
                                            .ThenInclude(a => a.Votes)  // Include answer votes
                                        .Include(d => d.Answers)
                                            .ThenInclude(a => a.Comments)
                                                .ThenInclude(c => c.User)  // Include comment user
                                        .Include(d => d.Answers)
                                            .ThenInclude(a => a.Comments)
                                                .ThenInclude(c => c.Votes)  // Include comment votes
                                        .Where(dis => dis.Id == id)
                                        .FirstOrDefault();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
            }

            SetAccessRights();

            //vote count
            int discussionTotalVotes = db.Votes.Count(vote => vote.DiscussionId == discussion.Id && vote.DidVote == 1) - db.Votes.Count(vote => vote.DiscussionId == discussion.Id && vote.DidVote == 2);
            discussion.NumberVotes = discussionTotalVotes;

            ApplicationUser currentUser = _userManager.GetUserAsync(User).Result;

            foreach (var answer in discussion.Answers) {
                int answerTotalVotes = db.Votes.Count(vote => vote.AnswerId == answer.Id && vote.DidVote == 1) - db.Votes.Count(vote => vote.AnswerId == answer.Id && vote.DidVote == 2);
                answer.ANumberVotes = answerTotalVotes;

                if (currentUser != null) {
                    Vote userVote = db.Votes.FirstOrDefault(vote => vote.AnswerId == answer.Id && vote.UserId == currentUser.Id);


                    if (userVote != null) {
                        answer.userVoted = userVote.DidVote;
                    } else {
                        answer.userVoted = 0; // User hasn't voted for this answer
                    }
                }
            }


            if (currentUser != null) {
                Vote existingVote = db.Votes.FirstOrDefault(v => v.DiscussionId == id && v.UserId == currentUser.Id);

                if (existingVote != null) {
                    ViewBag.HasVoted = existingVote.DidVote;
                } else {
                    ViewBag.HasVoted = 0;
                }
            }


            return View(discussion);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Upvote(int id) {


            ApplicationUser currentUser = _userManager.GetUserAsync(User).Result;

            Discussion discussion = db.Discussions.Include("User").Include("Answers").Include("Answers.User").Include("Answers.Comments").Include("Answers.Comments.User")
                        .Include(d => d.Votes).Where(dis => dis.Id == id)
                        .First();

            // Check if the user has already voted on this discussion
            Vote existingVote = db.Votes.FirstOrDefault(v => v.DiscussionId == id && v.UserId == currentUser.Id);

            if (existingVote != null) {
                // User has already voted, update the existing vote
                if (existingVote.DidVote == 1) { // we already upvoted, so we remove the vote
                    db.Votes.Remove(existingVote);
                    ViewBag.HasVoted = 0;
                } else {
                    db.Votes.Remove(existingVote);
                    Vote newVote = new Vote {
                        UserId = currentUser.Id,
                        DiscussionId = id,
                        AnswerId = null, // Set AnswerId based on the associated Answer
                        DidVote = 1 // Set to 1 for upvote
                    };
                    ViewBag.HasVoted = 1;
                    db.Votes.Add(newVote);
                }
            } else {
                Vote newVote = new Vote {
                    UserId = currentUser.Id,
                    DiscussionId = id,
                    AnswerId = null, // Set AnswerId based on the associated Answer
                    DidVote = 1 // Set to 1 for upvote
                };
                ViewBag.HasVoted = 1;
                db.Votes.Add(newVote);
            }

            db.SaveChanges();

            // Get the updated vote count
            int discussionTotalVotes = db.Votes.Count(vote => vote.DiscussionId == id && vote.DidVote == 1) - db.Votes.Count(vote => vote.DiscussionId == id && vote.DidVote == 2);

            // Set the updated vote count in ViewBag
            discussion.NumberVotes = discussionTotalVotes;

            // Redirect to  the discussion page or perform any other desired action
            return RedirectToAction("Show", new { id = id });
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Downvote(int id) {
            ApplicationUser currentUser = _userManager.GetUserAsync(User).Result;

            Discussion discussion = db.Discussions.Include("User").Include("Answers").Include("Answers.User").Include("Answers.Comments").Include("Answers.Comments.User")
                       .Include(d => d.Votes).Where(dis => dis.Id == id)
                       .First();
            // Check if the user has already voted on this discussion
            Vote existingVote = db.Votes.FirstOrDefault(v => v.DiscussionId == id && v.UserId == currentUser.Id);

            if (existingVote != null) {
                // User has already voted, update the existing vote
                if (existingVote.DidVote == 2) { // we already downvoted, so we remove the vote
                    db.Votes.Remove(existingVote);
                    ViewBag.HasVoted = 0;
                } else {
                    db.Votes.Remove(existingVote);
                    
                    Vote newVote = new Vote {
                        UserId = currentUser.Id,
                        DiscussionId = id,
                        AnswerId = null, // Set AnswerId based on the associated Answer
                        DidVote = 2 // Set to 2 for downvote
                    };
                    ViewBag.HasVoted = 2;
                    db.Votes.Add(newVote);
                }
            } else {
                // Check if the associated answer exists
                Vote newVote = new Vote {
                    UserId = currentUser.Id,
                    DiscussionId = id,
                    AnswerId = null, // Set AnswerId based on the associated Answer
                    DidVote = 2 // Set to 2 for downvote
                };
                ViewBag.HasVoted = 2;
                db.Votes.Add(newVote);
                
            }

            db.SaveChanges();

            // Get the updated vote count
            int discussionTotalVotes = db.Votes.Count(vote => vote.DiscussionId == id && vote.DidVote == 1) - db.Votes.Count(vote => vote.DiscussionId == id && vote.DidVote == 2);

            // Set the updated vote count in ViewBag
            discussion.NumberVotes = discussionTotalVotes;

            // Redirect to the discussion page or perform any other desired action
            return RedirectToAction("Show", new { id = id });
        }

        // Postare raspuns
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult AddAnswer([FromForm] Answer answer)
        {
            // TODO: eroarea e scrisa in mesaj
            // TODO: foloseste @Html.ValidationSummary

            answer.Date = DateTime.Now;
            answer.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Answers.Add(answer);
                db.SaveChanges();

                TempData["message"] = "Raspunsul a fost postat";
                TempData["messageType"] = "alert-success";
            }
            else
            {
                TempData["message"] = "Raspunsul trebuie sa aiba un continut";
                TempData["messageType"] = "alert-danger";
            }

            return Redirect("/Discussions/Show/" + answer.DiscussionId);
        }

        // Postare comentariu
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult AddComment([FromForm] Comment comment)
        {
            // TODO: eroarea e scrisa in mesaj
            // TODO: foloseste @Html.ValidationSummary

            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();

                TempData["message"] = "Comentariul a fost postat";
                TempData["messageType"] = "alert-success";
            }
            else
            {

                TempData["message"] = "Comentariul trebuie sa aiba un continut";
                TempData["messageType"] = "alert-danger";
            }

            Answer answer = db.Answers.Find(comment.AnswerId);
            return Redirect("/Discussions/Show/" + answer.DiscussionId);
        }

        // Editare discutie
        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Discussion discussion = db.Discussions
                                    .Where(dis => dis.Id == id)
                                    .First();

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
            discussion.CategoryId = categoryId;

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
            Discussion discussion = db.Discussions.Include("Answers").Include("Category").Include("Answers.Comments")
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

