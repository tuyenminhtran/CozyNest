using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CozyNest.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } // "Pending", "Shipping", "Delivered"
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}