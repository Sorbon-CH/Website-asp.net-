using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Gaestebuch_Sebastian.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Gaestebuch_Sebastian.Controllers
{
    public class AdminsController : Controller
    {
        private GaestebuchEntities db = new GaestebuchEntities();

        // GET: Admins
        public ActionResult Index()
        {
            // Benutzer wird zurück zur Hauptseite geleited, wenn er nicht eingeloggt ist
            if(Session["userID"] == null) { return RedirectToAction("index", "home"); }
            return View(db.Admin.ToList());
        }

        // GET: Admins/Details/5
        public ActionResult Details(int? id)
        {
            // Benutzer wird zurück zur Hauptseite geleited, wenn er nicht eingeloggt ist
            if (Session["userID"] == null) return RedirectToAction("index", "home"); 

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Admin admin = db.Admin.Find(id);

            if (admin == null) { return HttpNotFound(); }

            return View(admin);
        }

        // GET: Admins/Create
        public ActionResult Create()
        {
            // Benutzer wird zurück zur Hauptseite geleited, wenn er nicht eingeloggt ist
            if (Session["userID"] == null) { return RedirectToAction("index", "home"); }

            return View();
        }

        // POST: Admins/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Vorname,Password")] Admin admin)
        {
            if (Session["userID"] == null) { return RedirectToAction("index", "home"); }
            if (ModelState.IsValid)
            {
                admin.Password = HashPassword(admin.Password);

                // log wird erstellt und direkt in der Datenbank abgelegt.
                var log = new Logs();
                log.Aktion = "Admin " + admin.Vorname +" "+admin.Name + "created by Admin";
                log.Admin_ID = (int)Session["userID"];
                log.IP = Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString();
                log.Zeit = DateTime.Now;
                db.Logs.Add(log);

                db.Admin.Add(admin);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(admin);
        }

        // GET: Admins/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["userID"] == null) { return RedirectToAction("index", "home"); }

            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            Admin admin = db.Admin.Find(id);

            if (admin == null) { return HttpNotFound(); }

            return View(admin);
        }

        // POST: Admins/Edit/5
        // Aktivieren Sie zum Schutz vor Angriffen durch Overposting die jeweiligen Eigenschaften, mit denen eine Bindung erfolgen soll. 
        // Weitere Informationen finden Sie unter https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Vorname,Password")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                var log = new Logs();
                log.Aktion = "Admin " + admin.Vorname + " " + admin.Name + "edited by Admin";
                log.Admin_ID = (int)Session["userID"];
                log.IP = Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString();
                log.Zeit = DateTime.Now;
                db.Logs.Add(log);

                db.Entry(admin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(admin);
        }

        // GET: Admins/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["userID"] == null) { return RedirectToAction("index", "home"); }
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            Admin admin = db.Admin.Find(id);

            if (admin == null) {return HttpNotFound();}

            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Admin admin = db.Admin.Find(id);

            var log = new Logs();
            log.Aktion = "Admin " + admin.Vorname + " " + admin.Name + "deleted by Admin";
            log.Admin_ID = (int)Session["userID"];
            log.IP = Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString();
            log.Zeit = DateTime.Now;
            db.Logs.Add(log);

            db.Admin.Remove(admin);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: Admins/Login
        public ActionResult Login()
        {
            return View();
        }
        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Authorize(Admin admin)
        {
          
            using (GaestebuchEntities db = new GaestebuchEntities())
            {
                // Benutzer wird anhand des Namen und Vornamen gesucht
                var user = db.Admin.Where(x => x.Name == admin.Name && x.Vorname == admin.Vorname).FirstOrDefault();
               
                if (user == null)
                {
                    admin.LoginErrorMessage = "Name oder Vorname ist falsch";
                    return View("Login", admin);
                }
                else
                {
                    // Password wird überprüft
                    if (!VerifyHashedPassword(user.Password, admin.Password))
                    {
                        admin.LoginErrorMessage = "Password ist falsch";
                        return View("Login", admin);
                    }
                    else
                    {
                        // Log wird erstellt
                        var log = new Logs();
                        log.Aktion = "Admin " + admin.Vorname + " " + admin.Name + "Logged in";
                        log.Admin_ID = admin.Id;
                        log.IP = Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString();
                        log.Zeit = DateTime.Now;
                        db.Logs.Add(log);

                        // Session wird gesetzt
                        Session["userID"] = user.Id;
                        Session["userName"] = user.Name + " " + user.Vorname;
                        return RedirectToAction("index", "home");
                    }
                }
            }
            
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // Password Hash
        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        // Passwird überprüfung
        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return ByteArraysEqual(buffer3, buffer4);
        }
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }
    }
}
