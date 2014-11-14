using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RGAuditTool.Models;

namespace RGAuditTool.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        ApplicationDBContext db = new ApplicationDBContext();

        // GET: Employees
        public ActionResult Index(string supervisor)
        {
            //ViewBag.SupervisorId = new SelectList(db.Supervisors, "SupervisorId", "SupervisorName");
            //var emp = db.Employees.Include(a => a.Supervisors).Include(a => a.Clients);
            ViewBag.Sup = db.Employees.Select(e => e.Supervisors.SupervisorName).Distinct();
            //return View(emp.ToList());
            var model = db.Employees
                          .OrderByDescending(e => e.Supervisors.SupervisorId)
                          .Where(e => e.Supervisors.SupervisorName == supervisor || (supervisor == null));
            return View(model);
        }


        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }


        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
            ViewBag.SupervisorId = new SelectList(db.Supervisors, "SupervisorId", "SupervisorName");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "EmployeeId,EmployeeFirstName,EmployeeLastName,EmployeeEmail,EmployeeExtension,SupervisorId,EmployeeActiveStatus,ClientId,CreatedDate,ModifiedDate,CreatedBy,ModifiedBy")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
            ViewBag.SupervisorId = new SelectList(db.Supervisors, "SupervisorId", "SupervisorName");
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
            ViewBag.SupervisorId = new SelectList(db.Supervisors, "SupervisorId", "SupervisorName");
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "EmployeeId,EmployeeFirstName,EmployeeLastName,EmployeeEmail,EmployeeExtension,SupervisorId,EmployeeActiveStatus,ClientId,CreatedDate,ModifiedDate,CreatedBy,ModifiedBy")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "ClientName");
            ViewBag.SupervisorId = new SelectList(db.Supervisors, "SupervisorId", "SupervisorName");
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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
