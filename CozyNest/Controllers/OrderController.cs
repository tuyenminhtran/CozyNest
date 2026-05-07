using CozyNest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CozyNest.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ==================== CHECKOUT PAGE ====================
        [HttpGet]
        public ActionResult Checkout()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];
            var cart = db.Carts
                .Include("CartItems.Product")
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                return RedirectToAction("Index", "Cart");

            return View(cart);
        }

        // ==================== PLACE ORDER ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(string ShippingAddress)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(ShippingAddress))
            {
                ViewBag.Error = "Please enter a shipping address.";
                return RedirectToAction("Checkout");
            }

            int userId = (int)Session["UserId"];
            var cart = db.Carts
                .Include("CartItems.Product")
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                return RedirectToAction("Index", "Cart");

            // Tạo Order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                ShippingAddress = ShippingAddress,
                TotalAmount = cart.CartItems.Sum(i => i.Product.Price * i.Quantity)
            };

            db.Orders.Add(order);
            db.SaveChanges();

            // Tạo OrderItems từ CartItems
            foreach (var item in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                };
                db.OrderItems.Add(orderItem);

                // Giảm stock
                var product = db.Products.Find(item.ProductId);
                if (product != null && product.Stock >= item.Quantity)
                    product.Stock -= item.Quantity;
            }

            // Xóa Cart sau khi order
            db.CartItems.RemoveRange(cart.CartItems);
            db.SaveChanges();

            return RedirectToAction("OrderSuccess", new { orderId = order.OrderId });
        }

        // ==================== ORDER SUCCESS ====================
        public ActionResult OrderSuccess(int orderId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var order = db.Orders
                .Include("OrderItems.Product")
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null) return HttpNotFound();
            return View(order);
        }

        // ==================== MY ORDERS ====================
        public ActionResult MyOrders()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];
            var orders = db.Orders
                .Include("OrderItems.Product")
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}