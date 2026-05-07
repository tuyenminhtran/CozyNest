using CozyNest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CozyNest.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ==================== CHECK ADMIN ====================
        private bool IsAdmin()
        {
            return Session["UserId"] != null && Session["UserRole"] != null
                   && Session["UserRole"].ToString() == "Admin";
        }

        // ==================== DASHBOARD ====================
        public ActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            ViewBag.TotalProducts = db.Products.Count();
            ViewBag.TotalOrders = db.Orders.Count();
            ViewBag.TotalUsers = db.Users.Count();
            ViewBag.Revenue = db.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0;

            var recentOrders = db.Orders
                .Include("User")
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToList();

            ViewBag.RecentOrders = recentOrders;
            return View();
        }

        // ==================== PRODUCTS ====================
        public ActionResult Products(string search = "")
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var products = db.Products.Include("Category").AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                products = products.Where(p => p.Name.Contains(search));

            ViewBag.Search = search;
            ViewBag.Categories = db.Categories.ToList();
            return View(products.OrderByDescending(p => p.ProductId).ToList());
        }

        // ==================== CREATE PRODUCT ====================
        [HttpGet]
        public ActionResult CreateProduct()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            ViewBag.Categories = db.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(Product product)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(product.Name) || product.Price <= 0)
            {
                ViewBag.Error = "Please fill in all required fields.";
                ViewBag.Categories = db.Categories.ToList();
                return View(product);
            }

            db.Products.Add(product);
            db.SaveChanges();
            return RedirectToAction("Products");
        }

        // ==================== EDIT PRODUCT ====================
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var product = db.Products.Find(id);
            if (product == null) return HttpNotFound();

            ViewBag.Categories = db.Categories.ToList();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(Product product)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var existing = db.Products.Find(product.ProductId);
            if (existing == null) return HttpNotFound();

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.ImageUrl = product.ImageUrl;
            existing.Material = product.Material;
            existing.Stock = product.Stock;
            existing.CategoryId = product.CategoryId;

            db.SaveChanges();
            return RedirectToAction("Products");
        }

        // ==================== DELETE PRODUCT ====================
        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var product = db.Products.Find(id);
            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
            }

            return RedirectToAction("Products");
        }

        // ==================== ORDERS ====================
        public ActionResult Orders()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var orders = db.Orders
                .Include("User")
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        [HttpPost]
        public ActionResult UpdateOrderStatus(int orderId, string status)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var order = db.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = status;
                db.SaveChanges();
            }

            return RedirectToAction("Orders");
        }

        // ==================== USERS ====================
        public ActionResult Users()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var users = db.Users.OrderByDescending(u => u.CreatedAt).ToList();
            return View(users);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}