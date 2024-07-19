using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using IceCreamParlour.Models;
using IcecreamParlourSystem.Controllers;
using IcecreamParlourSystem.Models;

namespace IceCreamParlour.Models
{
    public class ItemsController : Controller
    {
        private IceCreamParlourEntities db = new IceCreamParlourEntities();
        // GET: Items
        public ActionResult Index()  
        {
            if (AccountController.IsUser || AccountController.IsLoggedIn==false)
                return RedirectToAction("ErrorAdmin","Home");
            return View(db.Items.ToList());
        }
        // GET: Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }
        // GET: Items/Create
        public ActionResult Create()
        {
            if (AccountController.IsUser || AccountController.IsLoggedIn==false)
                return RedirectToAction("ErrorAdmin", "Home");
            return View(); 
        }
        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ItemName,ItemFlavour,Price,Image,File")] Item item)
        {
            if (AccountController.IsUser || AccountController.IsLoggedIn==false)
                return RedirectToAction("ErrorAdmin","Home");
            if (ModelState.IsValid)
            {
                string filename = Path.GetFileName(item.File.FileName);
                string _filename = DateTime.Now.ToString("hhmmssfff") + filename;
                string path = Path.Combine(Server.MapPath("/Images/"),_filename);
                item.Image = "/Images/" + _filename;
                db.Items.Add(item);
                
                    if (db.SaveChanges() > 0)
                    {
                        item.File.SaveAs(path);
                    }
                    return RedirectToAction("Index");
                
            }
            return View(item);
        }
        // GET: Items/Edit/5
        public ActionResult Edit(int? id)
        {
            if (AccountController.IsUser || AccountController.IsLoggedIn==false)
                return RedirectToAction("ErrorAdmin", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            Session["imgPath"] = item.Image;
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }
        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ItemName,ItemFlavour,Price,Image,File")] Item item)
        {
            if (AccountController.IsUser || AccountController.IsLoggedIn == false)
                return RedirectToAction("ErrorAdmin", "Home");
            if (ModelState.IsValid)
            {
                if (item.File != null)
                {
                    string filename = Path.GetFileName(item.File.FileName);
                    string _filename = DateTime.Now.ToString("hhmmssfff") + filename;
                    string path = Path.Combine(Server.MapPath("~/Images/"), _filename);
                    item.Image = "/Images/" + _filename;
                    
                    if (item.File.ContentLength < 1000000)
                    {
                        db.Entry(item).State = EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            item.File.SaveAs(path);
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.msg = "File size should be less than 1MB";
                    }
                }
                else
                {
                    item.Image = Session["imgPath"].ToString();
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(item);
        }
        // GET: Items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (AccountController.IsUser || AccountController.IsLoggedIn == false)
                return RedirectToAction("ErrorAdmin","Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var ln = db.Items.FirstOrDefault(u=>u.Id==id);
            var order=db.orderts.Where(u=>u.item_id == ln.Id);
            db.orderts.RemoveRange(order);
            db.Items.Remove(ln);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Index1(string searching)
        {
            if (AccountController.IsAdmin)
                return RedirectToAction("ErrorAdmin", "Home");
            return View(db.Items.Where(x => x.ItemFlavour.Contains(searching) || searching == null).ToList());
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
