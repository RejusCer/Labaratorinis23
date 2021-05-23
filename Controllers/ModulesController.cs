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
    public class ModulesController : Controller
    {
        int N = 100; // leistinas skaičius
        private asplabEntities db = new asplabEntities();

        // GET: Modules
        public ActionResult Index()
        {
            if(Session["admin"].Equals(true))
            {
                return View(db.Modules.ToList());
            }

            int studID = Convert.ToInt32(Session["userID"]);
            var mods = db.ModuleUsers.Where(x => x.userID == studID).ToList(); // pamokos kurioje dalyvauja studentas

            List < Module > enrolledModules = new List<Module>(N);
            for (int j = 0; j < N; j++) enrolledModules.Add(null);

            foreach (var m in mods)
            {
                enrolledModules.Add(db.Modules.Find(m.moduleID));
            }

            enrolledModules.RemoveAll(item => item == null);
            return View(enrolledModules);
        }

        // GET: Modules/Details/5
        public ActionResult Details(int? id)
        {
            if (!ValidSession())
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }

        // GET: Modules/Create
        public ActionResult Create()
        {
            if (!ValidSession())
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "moduleID,moduleName,moduleDec")] Module module)
        {
            if (ModelState.IsValid)
            {
                db.Modules.Add(module);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(module);
        }

        // GET: Modules/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!ValidSession())
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "moduleID,moduleName,moduleDec")] Module module)
        {
            if (ModelState.IsValid)
            {
                db.Entry(module).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(module);
        }

        // GET: Modules/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!ValidSession())
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            List<Section> sections = db.Sections.Where(x => x.moduleID == id).ToList();
            List<ModuleUser> students = db.ModuleUsers.Where(x => x.moduleID == id).ToList();
            Module module = db.Modules.Find(id);

            foreach(var section in sections)
            {
                db.Sections.Remove(section);
            }
            foreach(var student in students)
            {
                db.ModuleUsers.Remove(student);
            }

            db.Modules.Remove(module);
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

        public ActionResult Module(int? id)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var sekcijos = db.Sections.Where(x => x.moduleID == id).ToList();
            if (sekcijos == null)
            {
                return HttpNotFound();
            }

            ViewBag.id = id;
            ViewBag.Title = db.Modules.Find(id).moduleName;
            ViewBag.Students = db.ModuleUsers.Where(x => x.moduleID == id).ToList();
            return View(sekcijos);
        }

        public ActionResult addStudents(int? id)
        {
            if (!ValidSession())
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var ModuleUsers = db.ModuleUsers.Where(x => x.moduleID == id).ToList(); // turimi
            var users = db.Users.Where(x => x.Administrator == false).ToList(); // visi

            int?[] TurimiID = new int?[N];
            List<User> u = new List<User>(N);
            for (int j = 0; j < N; j++) u.Add(null);

            int i = 0;
            bool detected = false;
            foreach (var moduleUser in ModuleUsers)
            {
                TurimiID[i] = moduleUser.User.userID;
                i++;
            }
            i = 0;

            foreach (var user in users)
            {
                detected = false;
                foreach (var item in TurimiID)
                {
                    if(item == user.userID && item != null)
                    {
                        detected = true;
                        break;
                    }
                }
                if (!detected)
                {
                    u[i] = user;
                    i++;
                }
            }

            u.RemoveAll(item => item == null);
            return View(u);
        }

        [HttpPost, ActionName("addStudents")]
        [ValidateAntiForgeryToken]
        public ActionResult addStudentsSave(int id)
        {
            string[] studentai = Request["MessageBox"].Split(' ');
            studentai = studentai.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            ModuleUser users = new ModuleUser();
            users.moduleID = id;

            foreach (var studentEmail in studentai)
            {
                var user = db.Users.Where(x => x.userEmail == studentEmail);
                foreach (var x in user)
                {
                    users.userID = x.userID;
                }
                db.ModuleUsers.Add(users);
                db.SaveChanges();
            }

            return RedirectToAction("Module", new { id });
        }

        public ActionResult deleteStudent(int id, int id2) // id modulio id, id2 studento id
        {
            if (!ValidSession())
            {
                return RedirectToAction("Index", "Home");
            }

            ModuleUser student = db.ModuleUsers.Find(id2);
            db.ModuleUsers.Remove(student);
            db.SaveChanges();

            return RedirectToAction("Module", new { id = id});
        }

        public ActionResult addGrade(int id, int id2)
        {
            if (!ValidSession())
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.id = id;
            return View(db.ModuleUsers.Find(id2));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addGrade([Bind(Include = "ID,userID,moduleID,grade")] ModuleUser stud, int id, int id2)
        {
            if(stud.grade <= 0 || stud.grade >= 10)
            {
                //ViewBag.error = "Pažymys turi būti tarp 0 ir 10";
                return RedirectToAction("addGrade", new { id, id2 });
            }

            if (ModelState.IsValid)
            {
                stud.ID = id2;
                db.Entry(stud).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Module", new { id });
            }

            return View();
        }

        public bool ValidSession()
        {
            if (Session["userID"] == null)
            {
                return false;
            }
            if (Session["admin"].Equals(false))
            {
                return false;
            }
            return true;
        }

    }
}
