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
    public class HomeController : Controller
    {
        private asplabEntities db = new asplabEntities();

        public ActionResult Index()
        {
            if (Session["userID"] == null)
            {
                return View();
            }
            return RedirectToAction("Index", "Modules");
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public ActionResult Login()
        {
            string email = Request["Email"];
            string password = Utils.hashPassword(Request["password"]);

            var user = db.Users.Where(x => x.userEmail.Equals(email) && x.userPassword.Equals(password) ).FirstOrDefault();

            if(user != null)
            {
                Session["userID"] = user.userID;
                Session["userName"] = user.userName;
                Session["admin"] = user.Administrator;

                return RedirectToAction("Index", "Modules");
            }

            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

    }
}