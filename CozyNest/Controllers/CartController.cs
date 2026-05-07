using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CozyNest.Models;
using System.Data.Entity;

namespace CozyNest.Controllers
{
    public class CartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ==================== VIEW CART ====================
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];
            var cart = db.Carts
                .Include("CartItems.Product")
                .FirstOrDefault(c => c.UserId == userId);

            return View(cart);
        }

        // ==================== ADD TO CART ====================
        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity = 1)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];

            var cart = db.Carts.FirstOrDefault(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                db.Carts.Add(cart);
                db.SaveChanges();
            }

            var existingItem = db.CartItems
                .FirstOrDefault(ci => ci.CartId == cart.CartId && ci.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = productId,
                    Quantity = quantity
                };
                db.CartItems.Add(cartItem);
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // ==================== REMOVE FROM CART ====================
        [HttpPost]
        public ActionResult RemoveFromCart(int cartItemId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var item = db.CartItems.Find(cartItemId);
            if (item != null)
            {
                db.CartItems.Remove(item);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // ==================== UPDATE QUANTITY ====================
        [HttpPost]
        public ActionResult UpdateQuantity(int cartItemId, int quantity)
        {
            if (Session["UserId"] == null)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            var item = db.CartItems.Find(cartItemId);
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}