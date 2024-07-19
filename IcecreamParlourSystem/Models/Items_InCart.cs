using IceCreamParlour.Models;
using IcecreamParlourSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IceCreamParlour.Models
{
    public class Items_InCart
    {
        public Item Product { get; set; }
        public loginNew user {  get; set; }
        public int Quantity { get; set; }
        public int TotalAmount { get; set; }
    }
}