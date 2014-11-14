using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RGAuditTool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RGAuditTool.Controllers
{
    [Authorize]
    public class AppUsersController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: AppUsers
        public ActionResult Index()
        {
            var appUsers = db.AppUsers.Include(a => a.UserGroup).Include(a => a.UserRole);
            //appUsers.OrderBy(s => s.LastName);
            return View(appUsers.OrderBy(s => s.LastName).Where(s => s.ActiveStatus == true));
        }

        public ActionResult InactiveUsers()
        {
            var appUsers = db.AppUsers.Include(a => a.UserGroup).Include(a => a.UserRole);
            var model = appUsers.OrderBy(s => s.LastName).Where(s => s.ActiveStatus == false);
            //appUsers.OrderBy(s => s.LastName);
            //return View(model);
            return PartialView(model);
        }

        // GET: AppUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.AppUsers.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // GET: AppUsers/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.GroupId = new SelectList(db.UserGroups, "GroupId", "GroupName");
            ViewBag.RoleId = new SelectList(db.UserRoles, "RoleId", "RoleName");
            return View();
        }

        // POST: AppUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "AppUserId,FirstName,LastName,Email,RoleId,GroupId,ActiveStatus,Password,AdminPermission,EditPermission,ViewPermission,CreatedDate,ModifiedDate,CreatedBy,ModifiedBy,UserName")] AppUser appUser, ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                db.AppUsers.Add(appUser);
                db.SaveChanges();
                if (appUser.RoleId == 4)
                {
                    var supName = string.Format("{0} {1}", appUser.FirstName, appUser.LastName);
                    var supervisor = new Supervisor() { SupervisorName = supName, SupervisorEmail = appUser.Email };
                    db.Supervisors.Add(supervisor);
                    db.SaveChanges();
                }
                var manager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(db));
                user = new ApplicationUser() { UserName = appUser.UserName, Email = appUser.Email, FirstName= appUser.FirstName, LastName = appUser.LastName, ActiveStatus = appUser.ActiveStatus };
                manager.Create(user, appUser.Password);
                return RedirectToAction("Index");
            }

            ViewBag.GroupId = new SelectList(db.UserGroups, "GroupId", "GroupName", appUser.GroupId);
            ViewBag.RoleId = new SelectList(db.UserRoles, "RoleId", "RoleName", appUser.RoleId);
            return View(appUser);
        }

        // GET: AppUsers/Edit/5
        public ActionResult Edit(int? id, string email)
        {
            if (id == null && email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.AppUsers.Find(id);

            var manager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(db));
            ApplicationUser user = manager.FindByEmail(email);

            if (appUser == null && user == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupId = new SelectList(db.UserGroups, "GroupId", "GroupName", appUser.GroupId);
            ViewBag.RoleId = new SelectList(db.UserRoles, "RoleId", "RoleName", appUser.RoleId);
            return View(appUser);
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Bind(Include = "AppUserId,FirstName,LastName,Email,RoleId,GroupId,ActiveStatus,Password,AdminPermission,EditPermission,ViewPermission,ModifiedDate,ModifiedBy,UserName")]
        [HttpPost]
        public ActionResult Edit(AppUser appUser, string email)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appUser).State = EntityState.Modified;
                db.SaveChanges();
                var manager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(db));
                ApplicationUser User = manager.FindByEmail(email);
                User.ActiveStatus = appUser.ActiveStatus;
                User.FirstName = appUser.FirstName;
                User.LastName = appUser.LastName;
                User.UserName = appUser.UserName;
                //db.Entry(User).State = EntityState.Modified;
                manager.UpdateAsync(User);
                //db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.GroupId = new SelectList(db.UserGroups, "GroupId", "GroupName", appUser.GroupId);
            ViewBag.RoleId = new SelectList(db.UserRoles, "RoleId", "RoleName", appUser.RoleId);
            return View(appUser);
        }

        // GET: AppUsers/Delete/5
        public ActionResult Delete(int? id, string email)
        {
            if (id == null && email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.AppUsers.Find(id);

            var manager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(db));
            ApplicationUser user = manager.FindByEmail(email);

            if (appUser == null && user == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // POST: AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id, string email)
        {
            AppUser appUser = db.AppUsers.Find(id);
            //db.AppUsers.Remove(appUser);
            appUser.ActiveStatus = false;
            db.SaveChanges();

            var manager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(db));
            ApplicationUser user = manager.FindByEmail(email);
            //var user = db.Users
            //           .Where(u => u.Email == email);
            //db.Users.Remove(user);
            user.ActiveStatus = false;
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
