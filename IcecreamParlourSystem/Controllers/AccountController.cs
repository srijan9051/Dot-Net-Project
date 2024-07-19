using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using IceCreamParlour.Models;
using IcecreamParlourSystem.Models;

namespace IcecreamParlourSystem.Controllers
{
    public class AccountController : Controller
    {
        private IceCreamParlourEntities db = new IceCreamParlourEntities();
        public static bool IsLoggedIn;
        public static bool IsAdmin=false;
        public static bool IsUser=false;
        // GET: Account
        public ActionResult Index()
        {
            if(IsLoggedIn && IsAdmin)
                return View(db.loginNews.ToList());
            return RedirectToAction("Login");
        }
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup([Bind(Include = "Id,UserName,Email,Password,ConfirmPassword")] loginNew lg)
        {
            if (db.loginNews.Any(x => x.UserName == lg.UserName))
            {
                ViewBag.Notification = "The account already exists";
                return View();
            }
            else
            {
                db.loginNews.Add(lg);
                db.SaveChanges();
                Session["Idss"] = lg.Id.ToString();
                Session["UserNamess"] = lg.UserName.ToString();
                IsLoggedIn = true;
                IsUser = true;
                return RedirectToAction("Index2", "Home");
            }
        }
        public ActionResult Logout()
        {
            // FormsAuthentication.SignOut();
            Session.Clear();
            IsLoggedIn = false;
            IsAdmin = false;
            IsUser = false;
            return RedirectToAction("Begin","Home");
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public ActionResult UserLogin()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(loginNew lg)
        {
            
            var checkLogin = db.loginNews.Where(x => x.UserName.Equals(lg.UserName) && x.Password.Equals(lg.Password)).FirstOrDefault();
            if (checkLogin != null)
            {
                Session["Idss"] = lg.Id.ToString();
                Session["UserNamess"] = lg.UserName.ToString();
                
                if (lg.UserName == "Admin" && lg.Password == "admin@123")
                {
                    IsLoggedIn= true;
                    IsAdmin = true;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    IsLoggedIn=false;
                    ViewBag.msg = "Not Authorized to login....you are not an Admin";
                }
            }
            else
            {
                ViewBag.Notification = "Wrong Username or Password";
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserLogin(loginNew lg)
        {
            var checkLogin = db.loginNews.Where(x => x.UserName.Equals(lg.UserName) && x.Password.Equals(lg.Password)).FirstOrDefault();
            if (checkLogin != null)
            {
                Session["Idss"] = lg.Id.ToString();
                Session["UserNamess"] = lg.UserName.ToString();
                if (lg.UserName == "Admin" && lg.Password == "admin@123")
                {
                    IsLoggedIn = false;
                    ViewBag.msg = "Not Authorized to login....you are not an Admin";
                    
                }
                else
                {
                    IsLoggedIn = true;
                    IsUser = true;
                    return RedirectToAction("Index2", "Home");
                }  
            }
            else
            {
                ViewBag.Notification = "Wrong Username or Password";
            }
            return View();
        }
        // GET: /User/Edit/5
        public ActionResult Edit(int id)
        {
            if (IsUser == true)
            {
                loginNew movie = db.loginNews.Find(id);
                return View(movie);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult Edit(loginNew movie)
        {
            movie.ConfirmPassword = movie.Password;
            var order = db.orderts.Where(o => o.cust_id == movie.Id).ToList();
            movie.orderts = order;           
            db.Entry(movie).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Editconfirmed");
        }
        public ActionResult EditConfirmed()
        {
            Session.Clear();
            IsLoggedIn = false;
            IsUser = false;
            return View();
        }
        public ActionResult Delete(string name)
        {
            if (IsUser == true)
            {
                loginNew movie = db.loginNews.FirstOrDefault(n=>n.UserName == name);
                return View(movie);
            }
            else
            {
                return HttpNotFound();
            }
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string name)
        {
            loginNew ln  = db.loginNews.FirstOrDefault(n => n.UserName == name);
            var order = db.orderts.Where(u => u.cust_id == ln.Id);
            db.orderts.RemoveRange(order);
            db.loginNews.Remove(ln);
            db.SaveChanges();
            IsUser = false;
            IsLoggedIn = false;
            Session.Clear();
            return RedirectToAction("Begin","Home");
        }
    }
}