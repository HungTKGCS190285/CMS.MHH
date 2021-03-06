    using CMS.MHH.Models;
using CMS.MHH.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace CMS.MHH.Controllers
{
    public class HomeController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();
        [Authorize(Roles = "Staff, QA Manager, QA_C, Admin")]
        public ActionResult Index(int page = 1, int pageSize = 5)
        {
            var idea_anony = db.Ideas.ToList();
            foreach (var a in idea_anony)
            {
                if (a.IsAnonymous == true)
                {
                    a.Author_Email = "Anonymous";
                }
                else
                {
                    a.Author_Email = a.Author.Email;
                }
                var cate = db.Categories.Find(a.CateId);
                a.CateName = cate.Category_Name;
            }
            var li_idea = idea_anony.OrderByDescending(x => x.Date).ToPagedList(page, pageSize);
            return View(li_idea);
        }

        public ActionResult MostViewed(int page = 1, int pageSize = 5)
        {
            var idea_most_viewed = db.Ideas.ToList();
            foreach (var a in idea_most_viewed)
            {
                if (a.IsAnonymous == true)
                {
                    a.Author_Email = "Anonymous";
                }
                else
                {
                    a.Author_Email = a.Author.Email;
                }
                var cate = db.Categories.Find(a.CateId);
                a.CateName = cate.Category_Name;
            }
            var li_idea_most_viewed = idea_most_viewed.OrderByDescending(x => x.View).Take(3).ToPagedList(page, pageSize);
            return View(li_idea_most_viewed);
        }

        public ActionResult _MostViewed()
        {
            var idea_most_viewed = db.Ideas.ToList();
            foreach (var a in idea_most_viewed)
            {
                if (a.IsAnonymous == true)
                {
                    a.Author_Email = "Anonymous";
                }
                else
                {
                    a.Author_Email = a.Author.Email;
                }
                var cate = db.Categories.Find(a.CateId);
                a.CateName = cate.Category_Name;
            }
            var li_idea_most_viewed = idea_most_viewed.OrderByDescending(x => x.View).Take(1);
            return PartialView(li_idea_most_viewed);
        }

        public ActionResult MostPopular(int page = 1, int pageSize = 5)
        {
            var idea_most_popular = db.Ideas.ToList();
            foreach (var a in idea_most_popular)
            {
                if (a.IsAnonymous == true)
                {
                    a.Author_Email = "Anonymous";
                }
                else
                {
                    a.Author_Email = a.Author.Email;
                }
                var cate = db.Categories.Find(a.CateId);
                a.CateName = cate.Category_Name;
            }
            var li_idea_most_popular = idea_most_popular.OrderByDescending(x => x.ThumbsUp).Take(3).ToPagedList(page, pageSize);
            return View(li_idea_most_popular);
        }

        public ActionResult _MostPopular()
        {
            var idea_most_popular = db.Ideas.ToList();
            foreach (var a in idea_most_popular)
            {
                if (a.IsAnonymous == true)
                {
                    a.Author_Email = "Anonymous";
                }
                else
                {
                    a.Author_Email = a.Author.Email;
                }
                var cate = db.Categories.Find(a.CateId);
                a.CateName = cate.Category_Name;
            }
            var li_idea_most_popular = idea_most_popular.OrderByDescending(x => x.ThumbsUp).Take(1);
            return PartialView(li_idea_most_popular);
        }

        [Authorize(Roles = "Staff, QA Manager, QA_C")]
        public ActionResult ViewDetail(int id)
        {
            Idea idea = db.Ideas.Find(id);
            idea.View++;
            db.Entry(idea).State = EntityState.Modified;
            db.SaveChanges();

            if(idea.IsAnonymous == true)
            {
                idea.Author_Email = "Anonymous";
            }
            else
            {
                idea.Author_Email = idea.Author.Email;
            }

            //count thumbs up in idea
            ViewBag.like = idea.ThumbsUp;

            //count thumbs up in idea
            ViewBag.Dislike = idea.ThumbsDown;

            //Get all users who react to this idea
            ViewBag.AllUserReacted = db.Reactions.Where(x => x.PostId == id ).ToList();


            var cmts = this.db.Comments.Include(x => x.Author).Where(x => x.Ideas.Id == id);

            var model = new IdeaVM()
            {
                Id = idea.Id,
                Content = idea.Content,
                Author = idea.Author_Email,
                CategoryName = idea.Cate.Category_Name,
                Date = idea.Date,
                View = idea.View,
                Title = idea.Title,
                Description = idea.Description,
                AuthorId = idea.AuthorId,
                DocumentName = idea.DocumentName,
                LastModify = idea.LastModify,

                comments = idea.Comments.OrderByDescending(r => r.Date).Select(r => new CommentVM()
                {
                    Comment = r.Text,
                    CommentAuthor = r.Author.Email,
                    CommentAnony = r.IsAnonymous,
                    CommentDate = r.Date
                }),
                
            };
            return View(model);
        }

        [Authorize(Roles = "Staff, QA Manager, QA_C")]
        public ActionResult Likes(int id, int status)
        {
           //execute the data and move to the view
            var result = Like(id, status);
            return Content(result);
        }

        // Create the function which execute the thumbs up and thumbs down action
        [Authorize(Roles = "Staff, QA Manager, QA_C")]
        public string Like(int id, int status)
        {
            var idea = db.Ideas.FirstOrDefault(x => x.Id == id);
            bool statu;
            // status = 1 - like; status = 2 - dislike
            if (status == 1)
            {
                statu = true;
            }
            else
            {
                statu = false;
            }

            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

            // Check the user reacted or not
            Reaction react = db.Reactions.FirstOrDefault(x => x.PostId == id && x.AuthorId == user.Id);
            
            if (react == null)
            {
                // if not, create a new react and add necessary information
                react = new Reaction();
                react.AuthorId = user.Id;

                //statu is bool (equal with IsLike)
                react.IsLike = statu;
                react.PostId = id;

                // used to add the number of react in the idea
                if (statu) // if status = true(like) 
                {
                    idea.ThumbsUp = idea.ThumbsUp + 1;
                }
                else
                {
                    idea.ThumbsDown = idea.ThumbsDown + 1;
                }
                db.Reactions.Add(react);
            }
            else // they reacted in the past
            {
                react.AuthorId = user.Id;
                react.IsLike = statu;
                react.PostId = id;

                if (statu)// if user like
                {
                    idea.ThumbsUp = idea.ThumbsUp + 1;

                    //check if the thumbs down is reacted by nobody
                    if (idea.ThumbsDown == 0 || idea.ThumbsDown < 0)
                    {
                        idea.ThumbsDown = 0;
                    }
                    else
                    {
                        idea.ThumbsDown = idea.ThumbsDown - 1;
                    }
                }
                else
                {
                    idea.ThumbsDown++;
                    //check if the thumbs up is reacted by nobody
                    if (idea.ThumbsUp == 0 || idea.ThumbsUp < 0)
                    {
                        idea.ThumbsUp = 0;
                    }
                    else
                    {
                        idea.ThumbsUp--;
                    }
                }
            }
            db.SaveChanges();
            return idea.ThumbsUp + "/" + idea.ThumbsDown;

        }

        [Authorize(Roles = "Staff, QA Manager, QA_C")]
        [HttpGet]
        public ActionResult CommentDetail(int id)
        {
            var com_anony = this.db.Comments.Include(x => x.Author).Where(x => x.Ideas.Id == id);
            foreach (var a in com_anony)
            {
                if (a.IsAnonymous == true)
                {
                    a.AuthorName = "Anonymous";
                }
                else
                {
                    a.AuthorName = a.Author.Email;
                }
            }
            com_anony = com_anony.OrderByDescending(x => x.Date);
            return this.PartialView("_AllComments", com_anony);
        }

        [Authorize(Roles = "Staff, QA Manager, QA_C")]
        [HttpGet]
        public ActionResult Comment(int id)
        {
            var check = db.Ideas.Find(id);
            var check_date = db.Submissions.Find(check.SubmissionId);

            if (DateTime.Now > check_date.Final_closure_date)
            {
                return RedirectToAction("Fail");
            }
            return this.PartialView("_Comment", new CMS.MHH.Models.Comment { IdeasId = id });
        }

        [Authorize(Roles = "Staff, QA Manager, QA_C")]
        public ActionResult Fail()
        {
            TempData["message"] = "The comment is out of final closure date, please check again";
            return View();
        }

        [Authorize(Roles = "Staff, QA Manager, QA_C")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Comment(Comment comment)
        {
            var idea = db.Ideas.Where(x => x.Id == comment.IdeasId).FirstOrDefault();
            var submit = db.Submissions.Find(idea.SubmissionId);
            if (DateTime.Now > submit.Closure_date)
            {
                TempData["message"] = "The submission is out of closure date, please submit other submission";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    comment.Date = DateTime.Now;
                    ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                    comment.AuthorId = user.Id;
                    db.Comments.Add(comment);
                    db.SaveChanges();


                    MailMessage mail = new MailMessage();

                    SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
                    smtpServer.Credentials = new System.Net.NetworkCredential("donotreply458@gmail.com", "<3333333");
                    smtpServer.Port = 587;
                    smtpServer.EnableSsl = true;

                    mail.From = new MailAddress("donotreply458@gmail.com");

                    var ideaID = comment.IdeasId;
                    var emailIdea = db.Ideas.Where(x => x.Id == ideaID).FirstOrDefault();
                    var email = emailIdea.Author.Email;

                    mail.To.Add(email);
                    mail.Subject = "Notification about new comment";
                    mail.Body = "A new comment has been post in your idea report";

                    smtpServer.Send(mail);
                    return RedirectToAction("Index", "Home");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }

            }
        }

        [Authorize(Roles = "Staff, QA Manager, QA_C")]
        [HttpGet]
        public ActionResult Comments(int id)
        {
            var check = db.Ideas.Find(id);
            var check_date = db.Submissions.Find(check.SubmissionId);

            if (DateTime.Now > check_date.Final_closure_date)
            {
                return RedirectToAction("Fail");
            }
            return this.PartialView("_Comments", new CMS.MHH.Models.Comment { IdeasId = id });
        }

        [Authorize(Roles = "Staff, QA Manager, QA_C")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Comments(Comment comment)
        {
            try
            {
                comment.Date = DateTime.Now;
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                comment.AuthorId = user.Id;
                db.Comments.Add(comment);
                db.SaveChanges();


                MailMessage mail = new MailMessage();

                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
                smtpServer.Credentials = new System.Net.NetworkCredential("donotreply458@gmail.com", "<3333333");
                smtpServer.Port = 587;
                smtpServer.EnableSsl = true;

                mail.From = new MailAddress("donotreply458@gmail.com");

                var ideaID = comment.IdeasId;
                var emailIdea = db.Ideas.Where(x => x.Id == ideaID).FirstOrDefault();
                var email = emailIdea.Author.Email;

                mail.To.Add(email);
                mail.Subject = "Notification about new comment";
                mail.Body = "A new comment has been post in your idea report";

                smtpServer.Send(mail);

                return RedirectToAction("ViewDetail", "Home", new { @id = ideaID });
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
        }
    }
}