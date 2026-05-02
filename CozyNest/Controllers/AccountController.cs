using CozyNest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CozyNest.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ==================== REGISTER ====================
        [HttpGet]
        public ActionResult Register()
        {
            if (Session["UserId"] != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string FullName, string Email, string Password, string ConfirmPassword)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ViewBag.Error = "Please fill in all fields.";
                return View();
            }

            if (Password != ConfirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            if (Password.Length < 6)
            {
                ViewBag.Error = "Password must be at least 6 characters.";
                return View();
            }

            // Check email tồn tại chưa
            if (db.Users.Any(u => u.Email == Email))
            {
                ViewBag.Error = "This email is already registered.";
                return View();
            }

            // Tạo user mới
            var user = new User
            {
                FullName = FullName,
                Email = Email,
                PasswordHash = HashPassword(Password),
                Role = "Customer",
                CreatedAt = DateTime.Now
            };

            db.Users.Add(user);
            db.SaveChanges();

            // Tạo cart cho user luôn
            var cart = new Cart { UserId = user.UserId };
            db.Carts.Add(cart);
            db.SaveChanges();

            // Auto login sau khi register
            Session["UserId"] = user.UserId;
            Session["UserName"] = user.FullName;
            Session["UserRole"] = user.Role;

            return RedirectToAction("Index", "Home");
        }

        // ==================== LOGIN ====================
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["UserId"] != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Email, string Password)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Error = "Please enter your email and password.";
                return View();
            }

            var hashedPassword = HashPassword(Password);
            var user = db.Users.FirstOrDefault(u => u.Email == Email && u.PasswordHash == hashedPassword);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            // Set session
            Session["UserId"] = user.UserId;
            Session["UserName"] = user.FullName;
            Session["UserRole"] = user.Role;

            // Redirect theo role
            if (user.Role == "Admin")
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Home");
        }

        // ==================== LOGOUT ====================
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        // ==================== HASH PASSWORD ====================
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sb = new StringBuilder();
                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}