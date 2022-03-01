using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CMS.MHH.Controllers
{
    public class RolesController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var Roles = db.Roles.ToList();
            return View(Roles);
        }

        public ActionResult Create()
        {
            var Role = new IdentityRole();
            return View(Role)   ;
        }

        [HttpPost]
        public ActionResult Create(IdentityRole Role)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var roleExist = roleManager.RoleExists(Role.Name);
            if (!roleExist)
            {
                db.Roles.Add(Role);
                db.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "The role has been exist in the system");
                return View(Role);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {            
            IdentityRole applicationUser = db.Roles.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            IdentityRole applicationUser = db.Roles.Find(id);
            db.Roles.Remove(applicationUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}