using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CozyNest.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}