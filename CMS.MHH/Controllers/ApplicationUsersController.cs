using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CMS.MHH.CustomFilters;
using CMS.MHH.Models;
using CMS.MHH.ViewModel;
using Microsoft.AspNet.Identity.Owin;

namespace CMS.MHH.Controllers
{
    [AuthLog(Roles = "Admin")]
    public class ApplicationUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ApplicationUsersController()
        {
            db = new ApplicationDbContext();
        }

        private ApplicationUserManager _userManager;
        public ApplicationUsersController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: ApplicationUsers
        public ActionResult Index()
        {
            var usersWithRoles = (from user in db.Users.Include(a => a.Department)
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email,
                                      DepartmentName = user.Department.Name,
                                      Phonenumber = user.PhoneNumber,
                                      RoleNames = (from userRole in user.Roles
                                                   join role in db.Roles on userRole.RoleId
                                                   equals role.Id
                                                   select role.Name).ToList()
                                  }).ToList().Select(p => new User_Role()

                                  {
                                      UserId = p.UserId,
                                      Username = p.Username,
                                      UserEmail = p.Email,
                                      Phonenumber = p.Phonenumber,
                                      Department = p.DepartmentName,
                                      UserRole = string.Join(",", p.RoleNames)
                                  });
            return View(usersWithRoles);
        }

        // GET: ApplicationUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name");
            ViewBag.Name = new SelectList(db.Roles.ToList(), "Name", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { Email = model.Email, UserName = model.Email, DepartmentId = model.DepartmentId, PhoneNumber = model.Phonenumber };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, model.Name);
                    return RedirectToAction("Index", "ApplicationUsers");
                }
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name", model.DepartmentId);
            return View(model);
        }

        // GET: ApplicationUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name", applicationUser.DepartmentId);
            ViewBag.Name = new SelectList(db.Roles.ToList(), "Name", "Name");
            return View(applicationUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    applicationUser.UserName = applicationUser.Email;
                    
                    var user = db.Users.Find(applicationUser.Id);

                    applicationUser.PasswordHash = user.PasswordHash;
                    applicationUser.SecurityStamp = user.SecurityStamp;

                    var roles = await UserManager.GetRolesAsync(user.Id);

                    await UserManager.RemoveFromRolesAsync(user.Id, roles.ToArray());

                    await UserManager.AddToRoleAsync(applicationUser.Id, applicationUser.Name);


                    db.Entry(user).CurrentValues.SetValues(applicationUser);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "Id", "Name", applicationUser.DepartmentId);
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
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
            ApplicationUser applicationUser = db.Users.Find(id);
            db.Users.Remove(applicationUser);
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
