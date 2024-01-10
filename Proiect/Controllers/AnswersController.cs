using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Data;
using Proiect.Data.Migrations;
using Proiect.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;

namespace Proiect.Controllers
{
    public class AnswersController : Controller {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public AnswersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Sterge din baza de date un raspuns postat
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Delete(int id) {
            Answer answer = db.Answers.Include("Comments")
                            .Where(ans => ans.Id == id)
                            .First();

            // sterge notificarile care aveau legatura cu aceast raspuns
            List<Notification> notifications = db.Notifications.Include("User")
                                               .Where(not => not.AnswerId == answer.Id)
                                               .ToList();

            foreach (Notification notification in notifications)
            {
                if (notification.Read == false)
                {
                    notification.User.UnreadNotifications--;
                }

                db.Notifications.Remove(notification);
            }

            Award awardToRemove = db.Awards.SingleOrDefault(a => a.AnswerId == id);
            if (awardToRemove != null) {
                Discussion discussion = db.Discussions.Find(awardToRemove.DiscussionId);
                if (discussion != null) {
                    discussion.didAward = null;
                }
                db.Awards.Remove(awardToRemove);
            }

            db.SaveChanges();

            // sterge manual toate comentariile de la acest raspuns
            foreach (Comment comment in answer.Comments) {
                db.Comments.Remove(comment);
            }

            db.SaveChanges();

            // verificam daca discutia ii apartine user-ului care incearca sa editeze /SAU/ daca este admin
            if (answer.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin")) {
                db.Answers.Remove(answer);
                db.SaveChanges();

                TempData["message"] = "Raspunsul a fost sters";
                TempData["messageType"] = "alert-success";
            } else {
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

            // verifica daca am votat deja
            Vote existingVote = db.Votes.FirstOrDefault(v => v.AnswerId == id && v.UserId == currentUser.Id);

            if (existingVote != null) {
                // userul a votat deja
                if (existingVote.DidVote == 1) { // aceeasi actiune => scoatem votul curent
                    db.Votes.Remove(existingVote);
                } else {
                    db.Votes.Remove(existingVote);
                    Vote newVote = new Vote {
                        UserId = currentUser.Id,
                        DiscussionId = null,
                        AnswerId = answer.Id,
                        DidVote = 1
                    };
                    db.Votes.Add(newVote);
                }
            } else {

                Vote newVote = new Vote {
                    UserId = currentUser.Id,
                    DiscussionId = null,
                    AnswerId = answer.Id,
                    DidVote = 1
                };
                db.Votes.Add(newVote);
            }

            db.SaveChanges();

            // numar voturi
            int answerTotalVotes = db.Votes.Count(vote => vote.DiscussionId == id && vote.DidVote == 1) - db.Votes.Count(vote => vote.DiscussionId == id && vote.DidVote == 2);

            answer.ANumberVotes = answerTotalVotes;

            Vote userVote = db.Votes.FirstOrDefault(vote => vote.AnswerId == answer.Id && vote.UserId == currentUser.Id);

            if (userVote != null) {
                answer.userVoted = userVote.DidVote;
            } else {
                answer.userVoted = 0; // nu a votat inca
            }

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

            // verifica daca am votat deja
            Vote existingVote = db.Votes.FirstOrDefault(v => v.AnswerId == id && v.UserId == currentUser.Id);

            if (existingVote != null) {
                // userul a votat deja
                if (existingVote.DidVote == 2) { // aceeasi actiune => scoatem votul curent
                    db.Votes.Remove(existingVote);
                } else {
                    db.Votes.Remove(existingVote);
                    Vote newVote = new Vote {
                        UserId = currentUser.Id,
                        DiscussionId = null,
                        AnswerId = answer.Id,
                        DidVote = 2
                    };
                    db.Votes.Add(newVote);
                }
            } else {
                Vote newVote = new Vote {
                    UserId = currentUser.Id,
                    DiscussionId = null,
                    AnswerId = answer.Id,
                    DidVote = 2
                };
                db.Votes.Add(newVote);

            }

            db.SaveChanges();

            // numar voturi
            int answerTotalVotes = db.Votes.Count(vote => vote.AnswerId == id && vote.DidVote == 1) - db.Votes.Count(vote => vote.AnswerId == id && vote.DidVote == 2);

            answer.ANumberVotes = answerTotalVotes;

            Vote userVote = db.Votes.FirstOrDefault(vote => vote.AnswerId == answer.Id && vote.UserId == currentUser.Id);

            if (userVote != null) {
                answer.userVoted = userVote.DidVote;
            } else {
                answer.userVoted = 0; // nu a votat inca
            }

            return Redirect("/Discussions/Show/" + answer.DiscussionId);
        }

        [Authorize(Roles = "User,Editor,Admin")]
        [HttpGet]
        public IActionResult Edit(int id) {
            Answer answer = db.Answers.Find(id);

            if (answer.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin")) {
                TempData["EditAnswerID"] = id;
                return Redirect("/Discussions/Show/" + answer.DiscussionId);
            } else {
                TempData["message"] = "Nu aveti dreptul sa editati un raspuns care nu va apartine"; ;
                TempData["messageType"] = "alert-success";

                return Redirect("/Discussions/Show/" + answer.DiscussionId);
            }
        }

        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public IActionResult GiveAward(int id) 
        {
            Answer answer = db.Answers.Include("User")
                            .Where(ans => ans.Id == id)
                            .First();

            Discussion discussion = db.Discussions.Find(answer.DiscussionId);
            var existingAward = db.Awards.FirstOrDefault(a => a.DiscussionId == discussion.Id);

            if (existingAward != null) {
                return Redirect("/Discussions/Show/" + answer.DiscussionId);
            }

            Award x = new Award { DidAward = false, DiscussionId = discussion.Id };

            if (answer.hasAward == null) {
                answer.hasAward = false;
            }

            if (discussion.didAward == null) {
                discussion.didAward = false;
            }

            if (answer.hasAward == false &&
                (User.HasClaim(ClaimTypes.Role, "Admin") || answer.UserId != _userManager.GetUserId(User)) &&
                discussion.didAward == false) {
                if (answer.UserId != null) {
                    x.UserId = answer.UserId;
                }
                if (answer.Id != null) {
                    x.AnswerId = answer.Id;
                }

                x.DidAward = true;
                discussion.didAward = true;
                answer.hasAward = true;
            }

            // adauga notificare catre utilizator care a primit award
            Notification NewNotification1 = new Notification
            {
                Read = false,
                DateMonth = DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture),
                DateDay = DateTime.Now.Day,
                UserId = answer.UserId,
                DiscussionId = answer.DiscussionId,
                AnswerId = answer.Id,
                Type = 3
            };

            // incrementeaza nr de notificari necitite ale user-ului
            answer.User.UnreadNotifications++;

            db.Notifications.Add(NewNotification1);

            db.Awards.Add(x);
            db.SaveChanges();

            return Redirect("/Discussions/Show/" + answer.DiscussionId);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult RemoveAward(int id) {
            Answer answer = db.Answers.Find(id);
            Discussion discussion = db.Discussions.Find(answer.DiscussionId);
            var existingAward = db.Awards.FirstOrDefault(a => a.DiscussionId == discussion.Id);
            if (answer.hasAward == true && User.HasClaim(ClaimTypes.Role, "Admin")) {
                discussion.didAward = null;
                answer.hasAward = false;
                db.Awards.Remove(existingAward);
            }

            // sterge notificarea cu award pt acest raspuns
            Notification notification = db.Notifications.Include("User")
                                        .Where(not => not.AnswerId == answer.Id && not.Type == 3)
                                        .First();

            if (notification.Read == false)
            {
                notification.User.UnreadNotifications--;
            }

            db.Notifications.Remove(notification);

            db.SaveChanges();

            return Redirect("/Discussions/Show/" + answer.DiscussionId);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Edit(int id, Answer requestAnswer)
        {
            var sanitizer = new HtmlSanitizer();

            Answer answer = db.Answers.Include("Comments").Include("Discussion").Include("Comments.User").Include("Discussion.User")
                            .Where(ans => ans.Id == id)
                            .First();

            requestAnswer.Date = DateTime.Now;

            if (answer.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    answer.Date = requestAnswer.Date;
                    requestAnswer.Content = sanitizer.Sanitize(requestAnswer.Content);
                    answer.Content = requestAnswer.Content;

                    db.SaveChanges();

                    // adauga si o notificare catre toti utilizatorii care au comentat la acest raspuns
                    Dictionary<string, bool> UserIds = new Dictionary<string, bool>();
                    foreach (Comment comment in answer.Comments)
                    {
                        if (answer.UserId != comment.UserId && !UserIds.ContainsKey(comment.UserId))
                        {
                            UserIds.Add(comment.UserId, true);

                            Notification NewNotification1 = new Notification
                            {
                                Read = false,
                                DateMonth = DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture),
                                DateDay = DateTime.Now.Day,
                                UserId = comment.UserId,
                                DiscussionId = answer.DiscussionId,
                                AnswerId = answer.Id,
                                Type = 5
                            };

                            // incrementeaza nr de notificari necitite ale user-ului
                            comment.User.UnreadNotifications++;

                            db.Notifications.Add(NewNotification1);
                            db.SaveChanges();
                        }
                    }

                    // notificare pentru cel care a initiat discutia 
                    Notification NewNotification2 = new Notification
                    {
                        Read = false,
                        DateMonth = DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture),
                        DateDay = DateTime.Now.Day,
                        UserId = answer.Discussion.UserId,
                        DiscussionId = answer.DiscussionId,
                        AnswerId = answer.Id,
                        Type = 6
                    };

                    // incrementeaza nr de notificari necitite ale user-ului
                    answer.Discussion.User.UnreadNotifications++;

                    db.Notifications.Add(NewNotification2);
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

