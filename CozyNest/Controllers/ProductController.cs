using CozyNest.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CozyNest.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string category = "")
        {
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.SelectedCategory = category;

            var products = db.Products.Include("Category").AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
                products = products.Where(p => p.Category.Name == category);

            return View(products.ToList());
        }

        public ActionResult Details(int id)
        {
            var product = db.Products.Include("Category").FirstOrDefault(p => p.ProductId == id);
            if (product == null) return HttpNotFound();

            ViewBag.RelatedProducts = db.Products
                .Where(p => p.CategoryId == product.CategoryId && p.ProductId != id)
                .Take(4).ToList();

            return View(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}