using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Concept.Data;
using Concept.Models;
using System.Security.Cryptography;
using System.Text;

namespace Concept.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to dashboard
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter username and password";
                return View();
            }

            // Hash the password
            string hashedPassword = HashPassword(password);

            // Find user by UserName (not UserCode)
            var user = await _context.UserInfos
                .Include(u => u.JobTitle)
                .FirstOrDefaultAsync(u => u.UserName == userName && u.UserPassword == hashedPassword && u.Active);

            if (user == null)
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }

            // Store user info in session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("UserCode", user.UserCode);

            return RedirectToAction("Index", "Home");
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // مسح جميع بيانات Session
            HttpContext.Session.Clear();

            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        // Password Hashing Method (SHA256)
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        // GET: Account/Profile
        public async Task<IActionResult> Profile()
        {
            // التحقق من تسجيل الدخول
            var userId = HttpContext.Session.GetInt32("UserId");  // جلب كـ Integer
            if (!userId.HasValue)  // التحقق من null
            {
                return RedirectToAction("Login");
            }
            // جلب بيانات المستخدم
            var user = await _context.UserInfos
                .Include(u => u.JobTitle)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == userId.Value);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found. Please login again.";
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: Account/ChangePassword
        public IActionResult ChangePassword()
        {
            // التحقق من تسجيل الدخول
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Please login to change your password.";
                return RedirectToAction("Login");
            }

            return View();
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            // التحقق من تسجيل الدخول
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Please login to change your password.";
                return RedirectToAction("Login");
            }

            // التحقق من وجود جميع الحقول
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.ErrorMessage = "All fields are required";
                return View();
            }

            // التحقق من تطابق كلمة المرور الجديدة
            if (newPassword != confirmPassword)
            {
                ViewBag.ErrorMessage = "New password and confirm password do not match";
                return View();
            }

            // التحقق من طول كلمة المرور
            if (newPassword.Length < 6)
            {
                ViewBag.ErrorMessage = "Password must be at least 6 characters";
                return View();
            }

            // جلب بيانات المستخدم
            var user = await _context.UserInfos.FindAsync(int.Parse(userId));
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found. Please login again.";
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            // التحقق من كلمة المرور الحالية
            if (user.UserPassword != currentPassword)
            {
                ViewBag.ErrorMessage = "Current password is incorrect";
                return View();
            }

            // التحقق من أن كلمة المرور الجديدة ليست نفس القديمة
            if (currentPassword == newPassword)
            {
                ViewBag.ErrorMessage = "New password must be different from current password";
                return View();
            }

            try
            {
                // تحديث كلمة المرور
                user.UserPassword = newPassword;
                user.ModifiedDate = DateTime.Now;

                _context.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing password: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while changing password. Please try again.";
                return View();
            }
        }
    }
}