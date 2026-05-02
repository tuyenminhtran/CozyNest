using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CozyNest.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}