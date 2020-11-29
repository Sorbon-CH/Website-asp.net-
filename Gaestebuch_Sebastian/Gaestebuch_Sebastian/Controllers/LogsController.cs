using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Gaestebuch_Sebastian.Models;

namespace Gaestebuch_Sebastian.Controllers
{
    public class LogsController : Controller
    {
        private GaestebuchEntities db = new GaestebuchEntities();

        // GET: Logs
        public ActionResult Index()
        {
            if (Session["userID"] == null) { return RedirectToAction("index", "home"); }
            var logs = db.Logs.Include(l => l.Admin);

            return View(logs.OrderByDescending(o => o.Zeit).ToList());
        }

        // GET: Logs/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["userID"] == null) { return RedirectToAction("index", "home"); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Logs logs = db.Logs.Find(id);
            if (logs == null)
            {
                return HttpNotFound();
            }
            return View(logs);
        }

        // GET: Logs/Create
        public ActionResult Create()
        {
            if (Session["userID"] == null) { return RedirectToAction("index", "home"); }
            ViewBag.Admin_ID = new SelectList(db.Admin, "Id", "Name");
            return View();
        }

        // POST: Logs/Create
        // Aktivieren Sie zum Schutz vor Angriffen durch Overposting die jeweiligen Eigenschaften, mit denen eine Bindung erfolgen soll. 
        // Weitere Informationen finden Sie unter https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Admin_ID,Aktion,Zeit,IP")] Logs logs)
        {
            if (ModelState.IsValid)
            {
                db.Logs.Add(logs);
                db.SaveChanges();
                return RedirectToAction("index", "home");
            }

            ViewBag.Admin_ID = new SelectList(db.Admin, "Id", "Name", logs.Admin_ID);
            return View(logs);
        }

        // GET: Logs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["userID"] == null) { return RedirectToAction("index", "home"); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Logs logs = db.Logs.Find(id);
            if (logs == null)
            {
                return HttpNotFound();
            }
            ViewBag.Admin_ID = new SelectList(db.Admin, "Id", "Name", logs.Admin_ID);
            return View(logs);
        }

        // POST: Logs/Edit/5
        // Aktivieren Sie zum Schutz vor Angriffen durch Overposting die jeweiligen Eigenschaften, mit denen eine Bindung erfolgen soll. 
        // Weitere Informationen finden Sie unter https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Admin_ID,Aktion,Zeit,IP")] Logs logs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(logs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("index", "home");
            }
            ViewBag.Admin_ID = new SelectList(db.Admin, "Id", "Name", logs.Admin_ID);
            return View(logs);
        }

        // GET: Logs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["userID"] == null) { return RedirectToAction("index", "home"); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Logs logs = db.Logs.Find(id);
            if (logs == null)
            {
                return HttpNotFound();
            }
            return View(logs);
        }

        // POST: Logs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Logs logs = db.Logs.Find(id);
            db.Logs.Remove(logs);
            db.SaveChanges();
            return RedirectToAction("index", "home");
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
