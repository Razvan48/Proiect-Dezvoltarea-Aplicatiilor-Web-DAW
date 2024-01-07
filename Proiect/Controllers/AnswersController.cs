using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            // TODO: nu sterge in cascada si comentariile

            Answer answer = db.Answers.Include("Comments")
                            .Where(ans => ans.Id == id)
                            .First();

            // verificam daca discutia ii apartine user-ului care incearca sa editeze /SAU/ daca este admin
            if (answer.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Answers.Remove(answer);
                db.SaveChanges();

                TempData["message"] = "Raspunsul a fost sters";
                TempData["messageType"] = "alert-success";
            }
            else
            {
                TempData["message"] = "Nu puteti sa stergeti raspunsul altor utilizatori";
                TempData["messageType"] = "alert-danger";
            }

            return Redirect("/Discussions/Show/" + answer.DiscussionId);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult UpvoteAnswer(int id) {
            ApplicationUser currentUser = _userManager.GetUserAsync(User).Result;

            Answer answer = db.Answers.Include(a => a.Discussion)
                                      .Include(a => a.Votes)
                                      .FirstOrDefault(a => a.Id == id);

            if (answer == null) {
                return BadRequest("Answer not found.");
            }

            // Check if the user has already voted on this discussion
            Vote existingVote = db.Votes.FirstOrDefault(v => v.AnswerId == id && v.UserId == currentUser.Id);

            if (existingVote != null) {
                // User has already voted, update the existing vote
                if (existingVote.DidVote == 1) { // we already upvoted, so we remove the vote
                    db.Votes.Remove(existingVote);
                } else {
                    db.Votes.Remove(existingVote);
                    Vote newVote = new Vote {
                        UserId = currentUser.Id,
                        DiscussionId = null,
                        AnswerId = answer.Id, // Set AnswerId based on the associated Answer
                        DidVote = 1 // Set to 1 for upvote
                    };
                    db.Votes.Add(newVote);
                }
            } else {    
                // User hasn't voted yet, create a new vote
                Vote newVote = new Vote {
                    UserId = currentUser.Id,
                    DiscussionId = null,
                    AnswerId = answer.Id, // Set AnswerId based on the associated Answer
                    DidVote = 1 // Set to 1 for upvote
                };
                db.Votes.Add(newVote);
            }

            db.SaveChanges();

            // Get the updated vote count
            int answerTotalVotes = db.Votes.Count(vote => vote.DiscussionId == id && vote.DidVote == 1) - db.Votes.Count(vote => vote.DiscussionId == id && vote.DidVote == 2);

            // Set the updated vote count in ViewBag
            answer.ANumberVotes = answerTotalVotes;

            Vote userVote = db.Votes.FirstOrDefault(vote => vote.AnswerId == answer.Id && vote.UserId == currentUser.Id);

            if (userVote != null) {
                answer.userVoted = userVote.DidVote;
            } else {
                answer.userVoted = 0; // User hasn't voted for this answer
            }

            // Redirect to  the discussion page or perform any other desired action
            return Redirect("/Discussions/Show/" + answer.DiscussionId);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult DownvoteAnswer(int id) {
            ApplicationUser currentUser = _userManager.GetUserAsync(User).Result;

            Answer answer = db.Answers.Include(a => a.Discussion)
                                      .Include(a => a.Votes)
                                      .FirstOrDefault(a => a.Id == id);

            if (answer == null) {
                return BadRequest("Answer not found.");
            }

            // Check if the user has already voted on this answer
            Vote existingVote = db.Votes.FirstOrDefault(v => v.AnswerId == id && v.UserId == currentUser.Id);

            if (existingVote != null) {
                // User has already voted, update the existing vote
                if (existingVote.DidVote == 2) { // we already downvoted, so we remove the vote
                    db.Votes.Remove(existingVote);
                } else {
                    db.Votes.Remove(existingVote);
                    Vote newVote = new Vote {
                        UserId = currentUser.Id,
                        DiscussionId = null,
                        AnswerId = answer.Id, // Set AnswerId based on the associated Answer
                        DidVote = 2 // Set to 2 for downvote
                    };
                    db.Votes.Add(newVote);
                }
            } else {
                Vote newVote = new Vote {
                    UserId = currentUser.Id,
                    DiscussionId = null,
                    AnswerId = answer.Id, // Set AnswerId based on the associated Answer
                    DidVote = 2 // Set to 2 for downvote
                };
                db.Votes.Add(newVote);
        
            }

            db.SaveChanges();

            // Get the updated vote count
            int answerTotalVotes = db.Votes.Count(vote => vote.AnswerId == id && vote.DidVote == 1) - db.Votes.Count(vote => vote.AnswerId == id && vote.DidVote == 2);

            // Set the updated vote count in ViewBag
            answer.ANumberVotes = answerTotalVotes;

            Vote userVote = db.Votes.FirstOrDefault(vote => vote.AnswerId == answer.Id && vote.UserId == currentUser.Id);

            if (userVote != null) {
                answer.userVoted = userVote.DidVote;
            } else {
                answer.userVoted = 0; // User hasn't voted for this answer
            }

            // Redirect to the discussion page or perform any other desired action
            return Redirect("/Discussions/Show/" + answer.DiscussionId);
        }

        [Authorize(Roles = "User,Editor,Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Answer answer = db.Answers.Find(id);

            if (answer.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                TempData["EditAnswerID"] = id;
                return Redirect("/Discussions/Show/" + answer.DiscussionId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati un raspuns care nu va apartine"; ;
                TempData["messageType"] = "alert-success";

                return Redirect("/Discussions/Show/" + answer.DiscussionId);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Edit(int id, Answer requestAnswer)
        {
            var sanitizer = new HtmlSanitizer();

            Answer answer = db.Answers.Find(id);

            if (answer.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    requestAnswer.Content = sanitizer.Sanitize(requestAnswer.Content);
                    answer.Content = requestAnswer.Content;

                    answer.Content = requestAnswer.Content;
                    db.SaveChanges();

                    TempData["message"] = "Raspunsul a fost editat";
                    TempData["messageType"] = "alert-success";

                    return Redirect("/Discussions/Show/" + answer.DiscussionId);
                }
                else
                {
                    TempData["message"] = "Continutul raspunsului nu poate fi null"; ;
                    TempData["messageType"] = "alert-danger";

                    return Redirect("/Answers/Edit/" + answer.Id);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati un raspuns care nu va apartine";
                TempData["messageType"] = "alert-danger";

                return Redirect("/Discussions/Show/" + answer.DiscussionId);
            }
        }
    }
}

