using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using CMS.MHH.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;

namespace CMS.MHH.Controllers
{
    public class IdeasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int page = 1, int pageSize = 5)
        {
            var ideas = db.Ideas.Include(i => i.Author).Include(i => i.Cate).Include(i => i.Submission).OrderByDescending(x => x.Id).ToPagedList(page, pageSize);
            return View(ideas);
        }

        public ActionResult MyIdeas(int page =1, int pageSize = 5)
        {
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var myidea = db.Ideas.Where(x => x.Author.Id == user.Id);
            if (myidea == null)
            {
                return RedirectToAction("Fail", "Home");
            }
            else
                return View(myidea.OrderByDescending(x => x.Date).ToPagedList(page, pageSize)); 
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Idea idea = db.Ideas.Find(id);
            if (idea == null)
            {
                return HttpNotFound();
            }
            return View(idea);
        }

        [HttpGet]
        public ActionResult Create(int id)
        {
            var submit = db.Submissions.Find(id);
            if (DateTime.Now > submit.Closure_date)
            {
                TempData["message"] = "The submission is out of closure date, please submit other submission";
                return RedirectToAction("List", "Submissions");
            }
            else
            {
                ViewBag.CateId = new SelectList(db.Categories, "Id", "Category_Name");
            }
            Idea newidea = new Idea() { SubmissionId = id };
            return View(newidea);
        }

        [HttpGet]
        public ActionResult Download(int id)
        {
            var ideas = this.db.Ideas.Where(x => x.Id == id).FirstOrDefault();
            string fullPath = Path.Combine(Server.MapPath("~/Files"), ideas.DocumentName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);


            return File(fileBytes, "application/pdf", ideas.DocumentName);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Idea idea)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                idea.AuthorId = user.Id;
                idea.Author_Email = user.Email;
                
                if (Request.Files != null && Request.Files.Count == 1)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(Server.MapPath("~/Files"), fileName);
                        file.SaveAs(filePath);
                        idea.DocumentName = fileName;
                    }
                }

                MailMessage mail = new MailMessage();

                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
                smtpServer.Credentials = new System.Net.NetworkCredential("leluongminhman2112@gmail.com", "0937757188");
                smtpServer.Port = 587;
                smtpServer.EnableSsl = true;

                mail.From = new MailAddress("leluongminhman2112@gmail.com");
                mail.To.Add("manllmgcs190101@fpt.edu.vn");
                mail.Subject = "Notification about new submitted idea";
                mail.Body = "A new idea has been posted in your department report";

                smtpServer.Send(mail);

                db.Ideas.Add(idea);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CateId = new SelectList(db.Categories, "Id", "Category_Name", idea.CateId);
            return View(idea);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Idea idea = db.Ideas.Find(id);
            if (idea == null)
            {
                return HttpNotFound();
            }
            ViewBag.CateId = new SelectList(db.Categories, "Id", "Category_Name", idea.CateId);
            return View(idea);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CateId,Title,Description,CateName,AuthorId,Author_Email,SubmissionId,IsAnonymous,Content,Date,DocumentName,View,ThumbsUp,ThumbsDown")] Idea idea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(idea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CateId = new SelectList(db.Categories, "Id", "Category_Name", idea.CateId);
            return View(idea);
        }

        // GET: Ideas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Idea idea = db.Ideas.Find(id);
            if (idea == null)
            {
                return HttpNotFound();
            }
            return View(idea);
        }

        // POST: Ideas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Idea idea = db.Ideas.Find(id);
            db.Ideas.Remove(idea);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
