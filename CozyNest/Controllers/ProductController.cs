using CozyNest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CozyNest.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            ViewBag.Categories = db.Categories.ToList();
            var products = db.Products.Include("Category").ToList();
            return View(products);
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