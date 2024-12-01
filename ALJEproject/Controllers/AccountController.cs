using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ALJEproject.Models; // Sesuaikan namespace berdasarkan struktur proyek Anda
using ALJEproject.ViewModels; // Anggap Anda memiliki LoginViewModel untuk detail login
using ALJEproject.Data;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;

namespace ALJEproject.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ALJEprojectDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        // Konstruktor yang menggabungkan kedua dependensi
        public AccountController(ALJEprojectDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel vm)
        {
            // Validasi input
            if (string.IsNullOrEmpty(vm.Username) || string.IsNullOrEmpty(vm.Password))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View(vm);
            }

            try
            {
                // Metode untuk validasi login
                if (SignInMethod(vm.Username, vm.Password))
                {
                    // Ambil data user dari database
                    var user = _context.UserRoles
                        .SingleOrDefault(u => u.UserName == vm.Username);

                    if (user != null)
                    {
                        // Set session untuk Username dan Role
                        HttpContext.Session.SetString("Username", user.UserName);
                        HttpContext.Session.SetString("Role", user.RoleName);

                        // Simpan hak akses menu berdasarkan Role ke dalam Session
                        var menuAccess = _context.UserAccessesView
                            .Where(ma => ma.RoleName == user.RoleName)
                            .Select(ma => new
                            {
                                ma.MenuName,
                                ma.Views,
                                ma.Inserts,
                                ma.Edits,
                                ma.Deletes
                            })
                            .ToList();

                        HttpContext.Session.Set("MenuAccess", System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(menuAccess));

                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError("", "User role data not found.");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                // Log error (misalkan dengan ILogger)
                Console.WriteLine($"Login error: {ex.Message}");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
            }

            // Jika gagal login
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Menghapus semua data session
            return RedirectToAction("Login", "Account");
        }

        private bool SignInMethod(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserName == username);

            if (user == null)
            {
                return false; // User not found
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success;
        }      
    }
}
