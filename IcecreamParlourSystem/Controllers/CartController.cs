using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using IceCreamParlour.Models;
using IcecreamParlourSystem.Controllers;
using IcecreamParlourSystem.Models;
using Newtonsoft.Json;

namespace SweetShop_MVC.Controllers
{
    public class CartController : Controller
    {
        private IceCreamParlourEntities db = new IceCreamParlourEntities();
        // GET: Cart
        public ActionResult Index()
        {
            if(AccountController.IsAdmin || AccountController.IsLoggedIn==false)
                return RedirectToAction("ErrorUser", "Home");
            return View();
        }
        public ActionResult finalChack()
        {
            if (AccountController.IsAdmin || AccountController.IsLoggedIn == false)
                return RedirectToAction("ErrorUser", "Home");
            return View();
        }
        [HttpPost]
        public ActionResult finalChack(string n)
        {
            return RedirectToAction("checkout");
        }
        public ActionResult AddToCart(int ProductID)
        {
            if (AccountController.IsAdmin || AccountController.IsLoggedIn == false)
                return RedirectToAction("ErrorUser", "Home");
            if (Session["cart"] == null)
            {
                List<Items_InCart> cart = new List<Items_InCart>();
                Items_InCart items_InCart = new Items_InCart();
                items_InCart.Product = db.Items.Find(ProductID);
                items_InCart.Quantity = 1;
                cart.Add(items_InCart);
                Session["cart"] = cart;
            }
            else
            {
                List<Items_InCart> cart = (List<Items_InCart>)Session["cart"];
                int index = IsInCart(ProductID);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new Items_InCart() { Product = db.Items.Find(ProductID), Quantity = 1 });
                }
                Session["cart"] = cart;
            }
            return RedirectToAction("Index");
        }
        public ActionResult RemoveFromCart(int ProductID)
        {
            if (AccountController.IsAdmin || AccountController.IsLoggedIn == false)
                return RedirectToAction("ErrorUser","Home");
            List<Items_InCart> cart = (List<Items_InCart>)Session["cart"];
            int index = IsInCart(ProductID);
            cart.RemoveAt(index);
            Session["cart"] = cart;
            return RedirectToAction("Index");
        }
        public int IsInCart(int ProductID)
        {
            List<Items_InCart> cart = (List<Items_InCart>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.Id == ProductID)
                {
                    return i;
                }
            }
            return -1;
        }
        public ActionResult checkout()
        {
            if (AccountController.IsAdmin || AccountController.IsLoggedIn == false)
                return RedirectToAction("ErrorUser","Home");

            return View();
        }
        public ActionResult Checkout1(string OrderData)
        {
            if (AccountController.IsAdmin || AccountController.IsLoggedIn == false)
                return RedirectToAction("ErrorUser","Home");
            var username = Session["Usernamess"];
            var userId = db.loginNews.First(u => u.UserName == username).Id;
            var orderData = JsonConvert.DeserializeObject<Dictionary<int, int>>(OrderData);
            loginNew lg = new loginNew();
            foreach (var item in orderData)
            {
                var order = db.Items.Find(item.Key);
                var price = int.Parse(order.Price);
                var quantity = item.Value;
                var orders = new ordert
                {
                    cust_id = userId,
                    item_id = item.Key,
                    price = price,
                    quantity = quantity,
                    totprice = price * quantity
                };
                db.orderts.Add(orders);
            }
            db.SaveChanges();
            return RedirectToAction("finalChack");
        }
        public ActionResult FinalOrders()
        {
            if (AccountController.IsUser || AccountController.IsLoggedIn == false)
                return RedirectToAction("ErrorAdmin", "Home");
            var order = db.orderts.ToList();
            return View(order);
        }
        public ActionResult OrderPast(string name)
        {
             var UserId = db.loginNews.First(u => u.UserName == name).Id;
            var order = db.orderts.Where(x=>x.cust_id==UserId);
            return View(order);
        }
    }
}
