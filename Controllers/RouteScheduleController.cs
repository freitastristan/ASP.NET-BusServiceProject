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
    public class RouteScheduleController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // list all start times for the selected bus route
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

            var routeSchedules = db.routeSchedules.Include(r => r.busRoute)
                .Where(a => a.busRouteCode == busRouteCode).OrderByDescending(a => a.isWeekDay).ThenBy(a=>a.startTime);
            return View(routeSchedules.ToList());
        }

        // produce the bus schedule for the given route/stop combination
        public ActionResult RouteStopSchedule(Int32? routeStopId)
        {
            if (routeStopId == null)
            {
                TempData["message"] = "please select a stop to see its schedule";
                return RedirectToAction("Index", "busStop");
            }

            var routeStop = db.routeStops.Find(routeStopId);
            if (routeStop == null)
            {
                TempData["message"] = "invalid route/stop combination - select a stop to see valid schedules";
                return RedirectToAction("Index", "busStop");
            }

            var stopSchedule = from record in db.routeSchedules
                               where record.busRouteCode == routeStop.busRouteCode
                               orderby record.isWeekDay descending, record.startTime
                               select record;
            foreach (var item in stopSchedule)
            {
                item.startTime = item.startTime.Add(TimeSpan.FromMinutes((double)routeStop.offsetMinutes));
            }
            var busRoute = db.busRoutes.Find(routeStop.busRouteCode);
            var busStop = db.busStops.Find(routeStop.busStopNumber);
            ViewBag.busRoute = busRoute.busRouteCode + " - " + busRoute.routeName;
            ViewBag.busStop = busStop.busStopNumber + " - " + busStop.location;
            return View(stopSchedule);
        }

        // show details for the selected schedule
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            return View(routeSchedule);
        }

        // return a blank schedule page to create a new schedule for this route
        public ActionResult Create()
        {
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName");
            return View();
        }

        // if the new schedule record passes edits, add it to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "routeScheduleId,busRouteCode,startTime,isWeekDay,comments")] routeSchedule routeSchedule)
        {
            if (ModelState.IsValid)
            {
                db.routeSchedules.Add(routeSchedule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        // return the selected schduel record for updating
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        // if the updated schedule record passes edits, update it on the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "routeScheduleId,busRouteCode,startTime,isWeekDay,comments")] routeSchedule routeSchedule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(routeSchedule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        // return the selected schedule to confirm delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            return View(routeSchedule);
        }

        // delete confirmed ... delete the schedule record
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            db.routeSchedules.Remove(routeSchedule);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // releaes resources for this session ... memory & connections
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
