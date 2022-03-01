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
    [Authorize(Roles = "Staff, QA Manager, QA_C")]
    public class HomeController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(int page = 1, int pageSize = 5)
        {
            var idea_anony = db.Ideas.ToList();
            foreach (var a in idea_anony)
            {
                if (a.IsAnonymous == true)
                {
                    a.Author_Email = "Anonymous";
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
                var cate = db.Categories.Find(a.CateId);
                a.CateName = cate.Category_Name;
            }
            var li_idea_most_viewed = idea_most_viewed.OrderByDescending(x => x.View).Take(3).ToPagedList(page, pageSize);
            return View(li_idea_most_viewed);
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
                var cate = db.Categories.Find(a.CateId);
                a.CateName = cate.Category_Name;
            }
            var li_idea_most_popular = idea_most_popular.OrderByDescending(x => x.ThumbsUp).Take(3).ToPagedList(page, pageSize);
            return View(li_idea_most_popular);
        }

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


            ViewBag.like = idea.ThumbsUp;
            ViewBag.Dislike = idea.ThumbsDown;
            ViewBag.AllUserlikedislike = db.Reactions.Where(x => x.PostId == id ).ToList();


            var model = new IdeaVM()
            {
                Id =  idea.Id,
                Content = idea.Content,
                Author = idea.Author_Email,
                CategoryName = idea.Cate.Category_Name,
                Date = idea.Date,
                View = idea.View,
                Title = idea.Title,
                Description = idea.Description,
                AuthorId = idea.AuthorId,

                comments = idea.Comments.Select(r => new CommentVM()
                { 
                    Comment = r.Text,
                    CommentAuthor = r.AuthorName,
                    CommentAnony = r.IsAnonymous,
                    CommentDate = r.Date
                }),              
                
            };
            return View(model);
        }

        public ActionResult Likes(int id, int status)
        {
            var result = Like(id, status);
            return Content(result);
        }

        public string Like(int id, int status)
        {
            var idea = db.Ideas.FirstOrDefault(x => x.Id == id);
            var toggle = false;
            bool statu;
            if (status == 1)
            {
                statu = true;
            }
            else
                statu = false;
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Reaction react = db.Reactions.FirstOrDefault(x => x.PostId == id && x.AuthorId == user.Id);
            if (react == null)
            {
                react = new Reaction();
                react.AuthorId = user.Id;

                react.IsLike = statu;
                react.PostId = id;
                if (statu)
                {
                    if (idea.ThumbsUp == null)
                    {
                        idea.ThumbsUp = 1;
                        idea.ThumbsDown = 0;
                    }
                    else
                    {
                        idea.ThumbsUp = idea.ThumbsUp + 1;
                    }
                }
                else
                {
                    if (idea.ThumbsDown == null)
                    {
                        idea.ThumbsDown = 1;
                        idea.ThumbsUp = 0;
                    }
                    else
                    {
                        idea.ThumbsDown = idea.ThumbsDown + 1;
                    }
                }
                db.Reactions.Add(react);
            }
            else
            {
                toggle = true;
            }
            if (toggle)
            {
                react.AuthorId = user.Id;
                react.IsLike = statu;
                react.PostId = id;
                if (statu)
                {
                    idea.ThumbsUp = idea.ThumbsUp + 1;
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
            return this.PartialView("_AllComments", com_anony);
        }

        [HttpGet]
        public ActionResult Comments(int id)
        {
            var check = db.Ideas.Find(id);
            var check_date = db.Submissions.Find(check.SubmissionId);

            if (DateTime.Now > check_date.Final_closure_date)
            {
                return RedirectToAction("Fail");
            }
            return this.PartialView("_Comment", new CMS.MHH.Models.Comment { IdeasId = id });
        }

        public ActionResult Fail()
        {
            TempData["message"] = "The comment is out of final closure date, please check again";
            return View();
        }

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
                smtpServer.Credentials = new System.Net.NetworkCredential("leluongminhman2112@gmail.com", "0937757188");
                smtpServer.Port = 587;
                smtpServer.EnableSsl = true;

                mail.From = new MailAddress("leluongminhman2112@gmail.com");
                mail.To.Add("manllmgcs190101@fpt.edu.vn");
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
}