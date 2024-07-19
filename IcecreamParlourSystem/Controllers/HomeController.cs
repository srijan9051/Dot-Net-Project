using IcecreamParlourSystem.Models;
using IceCreamParlour.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IcecreamParlourSystem.Controllers;

namespace IceCreamParlour.Controllers
{
    public class HomeController : Controller
    {
        public IceCreamParlourEntities db = new IceCreamParlourEntities();
        public static bool IsLoggedIn;


        public ActionResult Begin()
        {
            return View();
        }
        //[srijanAction]

        public ActionResult Index()
        {
            if (AccountController.IsLoggedIn && AccountController.IsAdmin == true)
                return View(db.Items.ToList());
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Index2()
        {

            if (AccountController.IsAdmin)
                return RedirectToAction("ErroeUser");
            var username = Session["Usernamess"];
            var userId = db.loginNews.First(u => u.UserName == username).Id;
            ViewBag.userId = userId;
            ViewBag.num = username;
            return View(db.Items.ToList());
        }

        public ActionResult About()
        {
            if (AccountController.IsUser || AccountController.IsLoggedIn == false)
                return RedirectToAction("ErrorAdmin");
            return View();
        }

        public ActionResult AboutMain()
        {

            return View();
        }

        public ActionResult Aboutu()
        {
            if (AccountController.IsAdmin || AccountController.IsLoggedIn == false)
                return RedirectToAction("ErrorUser");
            return View();
        }


        public ActionResult Contactu()
        {
            if (AccountController.IsAdmin || AccountController.IsLoggedIn == false)
                return RedirectToAction("ErrorUser");
            return View();
        }

        public ActionResult ErrorAdmin()
        {
            return View();
        }

        public ActionResult ErrorUser()
        {
            return View();
        }


    }
}