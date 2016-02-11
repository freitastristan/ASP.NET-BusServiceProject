using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using A4BusService.Models;

namespace A4BusService.Controllers
{
    public class DriverController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // GET: Driver
        public ActionResult Index()
        {
            var drivers = db.drivers.Include(d => d.province);
            return View(drivers.ToList());
        }

        // GET: Driver/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            driver driver = db.drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        // GET: Driver/Create
        public ActionResult Create()
        {
            ViewBag.provinceCode = new SelectList(db.provinces.OrderBy(a => a.name), "provinceCode", "name");
            return View();
        }

        // POST: Driver/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "driverId,firstName,lastName,fullName,homePhone,workPhone,street,city,postalCode,provinceCode,dateHired")] driver driver)
        {
            // GOOD FOR BOTH EDIT AND CREATE.
            try
            {
                if (ModelState.IsValid)
                {
                    db.drivers.Add(driver);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                while(ex.InnerException!=null) ex = ex.InnerException;
                ModelState.AddModelError("", "Error creating driver: " + ex.GetBaseException().Message); //Or .ToString
            }

            Create(); //Sorts viewbag for you
            return View(driver);
        }

        // GET: Driver/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            driver driver = db.drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            ViewBag.provinceCode = new SelectList(db.provinces, "provinceCode", "name", driver.provinceCode);
            return View(driver);
        }

        // POST: Driver/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "driverId,firstName,lastName,fullName,homePhone,workPhone,street,city,postalCode,provinceCode,dateHired")] driver driver)
        {
            if (ModelState.IsValid)
            {
                db.Entry(driver).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.provinceCode = new SelectList(db.provinces, "provinceCode", "name", driver.provinceCode);
            return View(driver);
        }

        // GET: Driver/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            driver driver = db.drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        // POST: Driver/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //This in try
            driver driver = db.drivers.Find(id);
            db.drivers.Remove(driver);
            db.SaveChanges();
            return RedirectToAction("Index");

            //catch innermost exception, the message, then call delete(no params) pass driverId [Tempdata] to show error message.
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
