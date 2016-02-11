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
    public class BusStopController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // GET: BusStop
        public ActionResult Index(string orderBy = "busStopNumber")
        {
            if (orderBy == "location")
                return View(db.busStops.OrderBy(a => a.busStopNumber));
            else
                return View(db.busStops.OrderBy(a=>a.busStopNumber));
        }

        // find all routes using the selected stop (if any)
        // - pass selected route/stop ID to the schedule controller
        public ActionResult RouteSelector(Int32 busStopNumber)
        {
            var routeStops = (from record in db.routeStops
                             where record.busStopNumber == busStopNumber
                             orderby record.busRouteCode
                             select new Int_and_String
                             {
                                 key = record.routeStopId,
                                 value = record.busRouteCode.ToString() + " - " + record.busRoute.routeName
                             }).ToList();
            if (routeStops.Count == 0)
            {
                TempData["message"] = "no routes use stop " + busStopNumber;
                return RedirectToAction("Index");
            }
            if (routeStops.Count == 1)
                return RedirectToAction("RouteStopSchedule", "RouteSchedule", new { routeStopId = routeStops[0].key });
            ViewBag.routeStopId = new SelectList(routeStops, "key", "value");
            ViewBag.busStopNumber = busStopNumber;
            return View();
        }


        // GET: BusStop/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        // GET: BusStop/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BusStop/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "busStopNumber,location,locationHash,goingDowntown")] busStop busStop)
        {
            if (ModelState.IsValid)
            {
                db.busStops.Add(busStop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(busStop);
        }

        // GET: BusStop/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        // POST: BusStop/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "busStopNumber,location,locationHash,goingDowntown")] busStop busStop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(busStop).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(busStop);
        }

        // GET: BusStop/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        // POST: BusStop/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            busStop busStop = db.busStops.Find(id);
            db.busStops.Remove(busStop);
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
