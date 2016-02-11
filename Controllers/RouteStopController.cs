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
    public class RouteStopController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // list all stops for the selected bus route, ordered by arrival time
        public ActionResult Index(string busRouteCode, string routeName)
        {
            if (busRouteCode != null)
            {
                Session["busRouteCode"] = busRouteCode;
                Session["routeName"] = routeName;
            }
            else if (Session["busRouteCode"] != null)
                busRouteCode = Session["busRouteCode"].ToString();
            else
            {
                TempData["message"] = "please select a route";
                return RedirectToAction("Index", "BusRoute");
            }

            var routeStops = db.routeStops.Include(r => r.busRoute).Include(r => r.busStop)
                .Where(a=>a.busRouteCode == busRouteCode).OrderBy(a=>a.offsetMinutes);
            return View(routeStops.ToList());
        }

        // Show details of the selected route/stop record
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeStop routeStop = db.routeStops.Find(id);
            if (routeStop == null)
            {
                return HttpNotFound();
            }
            return View(routeStop);
        }

        // return a page to attach a stop to the current route
        public ActionResult Create()
        {
            //ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName");
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location");
            return View();
        }

        // if the new route/stop relation passes eddits, add it to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "routeStopId,busRouteCode,busStopNumber,offsetMinutes")] routeStop routeStop)
        {
            if (ModelState.IsValid)
            {
                db.routeStops.Add(routeStop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeStop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routeStop.busStopNumber);
            return View(routeStop);
        }

        // modify an existing route/stop relation
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeStop routeStop = db.routeStops.Find(id);
            if (routeStop == null)
            {
                return HttpNotFound();
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeStop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routeStop.busStopNumber);
            return View(routeStop);
        }

        // if the updated route/stop relaton passes edits, update it on the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "routeStopId,busRouteCode,busStopNumber,offsetMinutes")] routeStop routeStop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(routeStop).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeStop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routeStop.busStopNumber);
            return View(routeStop);
        }

        // present a confirmation request to detach a stop from this route
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeStop routeStop = db.routeStops.Find(id);
            if (routeStop == null)
            {
                return HttpNotFound();
            }
            return View(routeStop);
        }

        // detach selected stop from this route
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            routeStop routeStop = db.routeStops.Find(id);
            db.routeStops.Remove(routeStop);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // release rousources for this session: memory & connections
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
