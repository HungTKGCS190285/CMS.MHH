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
            var myidea = db.Ideas.Where(x => x.Author.Id == user.Id).ToList();
            if (myidea == null)
            {
                return RedirectToAction("Fail", "Home");
            }
            else
            {
                foreach (var a in myidea)
                { 
                    var cate = db.Categories.Find(a.CateId);
                    a.CateName = cate.Category_Name;
                }
            }
            var li_idea = myidea.OrderByDescending(x => x.Date).ToPagedList(page, pageSize);

            return View(li_idea); 
        }

        public ActionResult Term_Condition()
        {
            return View();
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
            //Check the available to submit idea( base on the submission configuration

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
            // int id is for each idea.
            var ideas = this.db.Ideas.Where(x => x.Id == id).FirstOrDefault();

            //check the ideas has file or not
            string fullPath = Path.Combine(Server.MapPath("~/Files"), ideas.DocumentName);

            //open the file, read the content and export to byte format
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);

            //export the read file with the configuration format and this name 
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
                idea.Date = DateTime.Now;
                idea.LastModify = DateTime.Now;

                //check the idea contain file or not, in this case, just allow one document file for each idea.
                if (Request.Files != null && Request.Files.Count == 1)
                {
                    //Get the first submitted file
                    var file = Request.Files[0];

                    //check the valid file
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);

                        //prepare the storage where the file in with its name
                        var filePath = Path.Combine(Server.MapPath("~/Files"), fileName);
                        file.SaveAs(filePath);
                        idea.DocumentName = fileName;
                    }
                }

                //using available Mail in the system which have the information in the basic mail format 
                MailMessage mail = new MailMessage();

                // prepare the protocol for mail service, connect to the available mail,
                // in this case, using stmp of Google provider
                // set the config for transfer mail
                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
                smtpServer.Credentials = new System.Net.NetworkCredential("donotreply458@gmail.com", "<3333333");
                smtpServer.Port = 587;
                smtpServer.EnableSsl = true;


                //Config information for the mail service
                mail.From = new MailAddress("donotreply458@gmail.com");
                if (user.Department.Name == "HR")
                {
                    mail.To.Add("qahr789@gmail.com");
                }
                else if (user.Department.Name == "IT")
                {
                    mail.To.Add("qait321@gmail.com");
                }
                else if (user.Department.Name == "QA")
                {
                    mail.To.Add("qamanager321@gmail.com");
                }
                mail.Subject = "Notification about new submitted idea";
                mail.Body = "A new idea has been posted in your department";

                //send the mail in smtp protocol
                smtpServer.Send(mail);

                db.Ideas.Add(idea);
                db.SaveChanges();
                return RedirectToAction("Index","Home");
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
            var date_check = db.Submissions.Find(idea.SubmissionId);
            if(DateTime.Now > date_check.Closure_date)
            {
                TempData["message"] = "Can not edit the post because it is out of date";
                return RedirectToAction("Index", "Home");
            }
            if (idea == null)
            {
                return HttpNotFound();
            }
            ViewBag.CateId = new SelectList(db.Categories, "Id", "Category_Name", idea.CateId);
            return View(idea);
        }   
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Idea idea)
        {
             if (ModelState.IsValid)
            {
                
                if (Request.Files != null && Request.Files.Count == 1)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(Server.MapPath("~/Files"), fileName);

                        var idea1 = db.Ideas.FirstOrDefault(x => x.Id == idea.Id);
                        if(idea1.DocumentName == null)
                        {
                            file.SaveAs(filePath);
                        }
                        else
                        {
                            var fi = idea1.DocumentName;
                            var fiPath = Path.Combine(Server.MapPath("~/Files"), fi);
                            fi.Replace(fiPath, filePath);
                        }                        
                        idea.DocumentName = fileName;
                    }
                }

                var existingEntity = db.Ideas.Find(idea.Id);
                {
                    idea.Date = existingEntity.Date;
                    idea.Comments = existingEntity.Comments;
                    idea.ThumbsDown = existingEntity.ThumbsDown;
                    idea.ThumbsUp = existingEntity.ThumbsUp;
                    idea.LastModify = DateTime.Now;
                    idea.View = existingEntity.View;
                    idea.AuthorId = existingEntity.AuthorId;
                }               
                
                db.Entry(existingEntity).CurrentValues.SetValues(idea);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
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
