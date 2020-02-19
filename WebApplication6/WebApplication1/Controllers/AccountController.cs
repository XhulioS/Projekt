using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Web.Security;
using System.Security.Cryptography;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.Table model)
        {
            using (var context = new MyContactBookEntities3())
            {
                var user = from u in context.Tables
                           where u.Username == model.Username
                           select u;

                if (user.ToList().Count == 1)
                {
                    if (user.First().Password == CreatePasswordHash(model.Password))
                    {
                        FormsAuthentication.SetAuthCookie(model.Username, false);
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Invalid username and password");
                if (!ModelState.IsValid)
                    return View();
                return RedirectToAction("Login", "Home");

            }
        }


        private static string CreatePasswordHash(string password)
        {
            string passwrodSalt = String.Concat(password);
            string hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(passwrodSalt,"SHA1");
            return hashedPwd;
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(Table model)
        {
            using (var context = new MyContactBookEntities3())
            {
                model.Password = CreatePasswordHash(model.Password);
                context.Tables.Add(model);
                context.SaveChanges();
            }
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}