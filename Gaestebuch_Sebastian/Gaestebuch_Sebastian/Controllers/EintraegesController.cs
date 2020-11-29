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
    
    public class EintraegesController : Controller
    {

        private GaestebuchEntities db = new GaestebuchEntities();

        // GET: Eintraeges
        public ActionResult Index()
        {
            // Wenn der Benutzer nicht eingelogt ist, werden ihm nur die Autorisierten Einträge angezeigt.
            if(Session["userName"] == null)
            {
                var eintraege = db.Eintraege.Where(e => e.Autorisiert_ID != null);
                return View(eintraege.OrderByDescending(o => o.Besuchsdatum).Take(20).ToList());
            }
            else
            {
                var eintraege = db.Eintraege.Include(e => e.Admin);
                return View(eintraege.ToList());
            }

        }

        public ActionResult archiv()
        {
            // Hier werden im gegensatz zum Index alle Einträge angezeigt
            if (Session["userName"] == null)
            {
                var eintraege = db.Eintraege.Where(e => e.Autorisiert_ID != null);
                return View(eintraege.OrderByDescending(o => o.Besuchsdatum).ToList());
            }
            else
            {
                var eintraege = db.Eintraege.Include(e => e.Admin);
                return View(eintraege.ToList());
            }

        }

        // GET: Eintraeges/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eintraege eintraege = db.Eintraege.Find(id);
            if (eintraege == null)
            {
                return HttpNotFound();
            }
            return View(eintraege);
        }
        
        // GET: Eintraeges/Create
        public ActionResult Create()
        {
            ViewBag.Autorisiert_ID = new SelectList(db.Admin, "Id", "Name");

            // hollt IP
            ViewBag.userIP = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString();

            return View();
        }

        // POST: Eintraeges/Create
        // Aktivieren Sie zum Schutz vor Angriffen durch Overposting die jeweiligen Eigenschaften, mit denen eine Bindung erfolgen soll. 
        // Weitere Informationen finden Sie unter https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IP,Name,Besuchsdatum,Kommentar,Autorisiert_ID")] Eintraege eintraege)
        {
            if (ModelState.IsValid)
            {
                // log eintrag erstellen
                var log = new Logs();
                log.Aktion = "Created Entry by " + eintraege.Name;
                log.IP = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString();
                log.Zeit = DateTime.Now;
                db.Logs.Add(log);

                db.Eintraege.Add(eintraege);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Autorisiert_ID = new SelectList(db.Admin, "Id", "Name", eintraege.Autorisiert_ID);
            return View(eintraege);
        }

        public ActionResult Autorisieren(int? id)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Index");
            }
            else
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var log = new Logs();
                log.Aktion = "Entry with ID " + id + "authorized by Admin";
                log.Admin_ID = (int)Session["userID"];
                log.IP = Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString();
                log.Zeit = DateTime.Now;
                db.Logs.Add(log);

                Eintraege eintraege = db.Eintraege.Find(id);
                db.Eintraege.Find(id).Autorisiert_ID = (int)Session["userID"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        // GET: Eintraeges/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["userID"] == null) { return RedirectToAction("index"); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eintraege eintraege = db.Eintraege.Find(id);
            if (eintraege == null)
            {
                return HttpNotFound();
            }
            ViewBag.Autorisiert_ID = new SelectList(db.Admin, "Id", "Name", eintraege.Autorisiert_ID);
            return View(eintraege);
        }

        // POST: Eintraeges/Edit/5
        // Aktivieren Sie zum Schutz vor Angriffen durch Overposting die jeweiligen Eigenschaften, mit denen eine Bindung erfolgen soll. 
        // Weitere Informationen finden Sie unter https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IP,Name,Besuchsdatum,Kommentar,Autorisiert_ID")] Eintraege eintraege)
        {
            if (ModelState.IsValid)
            {

                var log = new Logs();
                log.Aktion = "Entry with ID " + eintraege.Id + " edited by Admin";
                log.Admin_ID = (int)Session["userID"];
                log.IP = Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString();
                log.Zeit = DateTime.Now;
                db.Logs.Add(log);

                db.Entry(eintraege).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Autorisiert_ID = new SelectList(db.Admin, "Id", "Name", eintraege.Autorisiert_ID);
            return View(eintraege);
        }

        // GET: Eintraeges/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["userID"] == null) { return RedirectToAction("index"); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eintraege eintraege = db.Eintraege.Find(id);
            if (eintraege == null)
            {
                return HttpNotFound();
            }
            return View(eintraege);
        }

        // POST: Eintraeges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            var log = new Logs();
            log.Aktion = "Entry with ID " + id + "deleted by Admin";
            log.Admin_ID = (int)Session["userID"];
            log.IP = Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString();
            log.Zeit = DateTime.Now;
            db.Logs.Add(log);

            Eintraege eintraege = db.Eintraege.Find(id);
            db.Eintraege.Remove(eintraege);
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
