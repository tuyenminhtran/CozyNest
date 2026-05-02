using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CozyNest.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // "Admin" hoặc "Customer"
        public DateTime CreatedAt { get; set; }
    }
}