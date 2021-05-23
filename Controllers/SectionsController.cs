using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Labaratorinis23.Models;

namespace Labaratorinis23.Controllers
{
    public class SectionsController : Controller
    {
        private asplabEntities db = new asplabEntities();

        // GET: Sections
        public ActionResult Index()
        {
            var sections = db.Sections.Include(s => s.Module);
            return View(sections.ToList());
        }

        // GET: Sections/Create
        public ActionResult Create(int id)
        {
            ViewBag.id = id;
            return View();
        }

        // POST: Sections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "sectionID,sectionName,sectionDec,moduleID")] Section section, int id)
        {
            section.moduleID = id;
            if (ModelState.IsValid)
            {
                db.Sections.Add(section);
                db.SaveChanges();
                return RedirectToAction("Module", "Modules", new { id });
            }

            ViewBag.moduleID = new SelectList(db.Modules, "moduleID", "moduleName", section.moduleID);
            return View(section);
        }

        // GET: Sections/Edit/5
        public ActionResult Edit(int id, int? id2)
        {
            if (id2 == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Section section = db.Sections.Find(id2);
            if (section == null)
            {
                return HttpNotFound();
            }
            //ViewBag.moduleID = new SelectList(db.Modules, "moduleID", "moduleName", section.moduleID);
            ViewBag.id = id;
            return View(section);
        }

        // POST: Sections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "sectionID,sectionName,sectionDec,moduleID")] Section section, int id)
        {

            if (ModelState.IsValid)
            {
                db.Entry(section).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Module", "Modules", new { id });
            }
            ViewBag.moduleID = new SelectList(db.Modules, "moduleID", "moduleName", section.moduleID);
            return View(section);
        }

        // GET: Sections/Delete/5
        public ActionResult Delete(int id, int? id2)
        {
            if (id2 == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Section section = db.Sections.Find(id2);
            if (section == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = id;
            return View(section);
        }
        
        // POST: Sections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int id2)
        {
            Section section = db.Sections.Find(id2);
            db.Sections.Remove(section);
            db.SaveChanges();
            return RedirectToAction("Module", "Modules", new { id });
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
