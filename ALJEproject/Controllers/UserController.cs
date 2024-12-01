using ALJEproject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using ALJEproject.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using ALJEproject.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using ClosedXML.Excel;

namespace ALJEproject.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly ALJEprojectDbContext _context;
        private readonly IUserService _userService;


        // Konstruktor yang menggabungkan kedua dependensi
        public UserController(IUserService userService, ALJEprojectDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
        }

        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var menus = await _userService.GetActiveMenusAsync();
            ViewBag.Menus = menus;
            IEnumerable<UserRoleView> users;
            int totalUsers; // Declare a variable for total user count

            if (!string.IsNullOrWhiteSpace(search))
            {
                users = _userService.SearchUsers(search, page, pageSize);
                // Get the total count of users matching the search criteria
                totalUsers = _userService.GetTotalUsersCount(search); // Pass the search term to count matching users
            }
            else
            {
                users = _userService.GetPaginatedUsers(page, pageSize);
                totalUsers = _userService.GetTotalUsersCount(); // Get the total user count without filtering
            }

            var model = new PaginatedUserViewModel
            {
                Users = users,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalUsers // This will now reflect the correct count based on search
            };           

            ViewData["CurrentSearch"] = search; // Save search term to display in the view

            return View(model);
        }



        [HttpGet]
        public IActionResult Create()
        {
            var roles = _context.Roles.Select(r => new { r.RoleID, r.RoleName }).ToList();
            ViewBag.RoleList = new SelectList(roles, "RoleID", "RoleName");

            return PartialView("_CreateUserPartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                var passwordHasher = new PasswordHasher<User>();
                user.Password = passwordHasher.HashPassword(user, user.Password);

                user.CreatedDate = DateTime.Now;
                user.CreatedBy = user.UserName;
                _context.Users.Add(user);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = true });
        }

        // GET: User/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = _context.Roles.Select(r => new { r.RoleID, r.RoleName }).ToList();
            ViewBag.RoleList = new SelectList(roles, "RoleID", "RoleName", user.RoleID);

            return PartialView("_EditUserPartial", user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    // Memperbarui entitas User di konteks
                    var passwordHasher = new PasswordHasher<User>();
                    user.Password = passwordHasher.HashPassword(user, user.Password);

                    user.UpdatedDate = DateTime.Now;
                    user.UpdatedBy = user.UserName;
                    _context.Update(user);
                    _context.SaveChanges();
                    _logger.LogInformation("User with ID {UserId} updated successfully.", user.UserId);
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    // Menangani kesalahan saat menyimpan ke database
                    _logger.LogError(ex, "An error occurred while updating user with ID {UserId}.", user.UserId);
                    return Json(new { success = false, message = "An error occurred while updating the user." });
                }
            }

            // Jika model tidak valid, kembalikan error ke klien
            var errorMessages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            _logger.LogError("Failed to update user with ID {UserId}. Model state is invalid.", user.UserId);

            // If the model state is not valid, repopulate the roles dropdown
            var roles = _context.Roles.Select(r => new { r.RoleID, r.RoleName }).ToList();
            ViewBag.RoleList = new SelectList(roles, "RoleID", "RoleName", user.RoleID);

            return Json(new { success = false, errors = errorMessages });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                _logger.LogInformation("User with ID {UserId} deleted successfully.", id);
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = new[] { "User not found." } });
        }

        public IActionResult View(int id)
        {
            var user = _context.UserRoles.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            return PartialView("_ViewUserPartial", user); // Use a partial view for the modal content
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        public IActionResult ExportToExcel()
        {
            // Assuming you have a method to get user data
            var users = _context.UserRoles.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Users");
                worksheet.Cell(1, 1).Value = "User ID";
                worksheet.Cell(1, 2).Value = "User Name";
                worksheet.Cell(1, 3).Value = "Full Name";
                worksheet.Cell(1, 4).Value = "Email Address";
                worksheet.Cell(1, 5).Value = "Role";

                for (int i = 0; i < users.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = users[i].UserId;
                    worksheet.Cell(i + 2, 2).Value = users[i].UserName;
                    worksheet.Cell(i + 2, 3).Value = users[i].FullName;
                    worksheet.Cell(i + 2, 4).Value = users[i].EmailAddress;
                    worksheet.Cell(i + 2, 5).Value = users[i].RoleName; // Adjust if using a role ID or name
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Users.xlsx");
                }
            }
        }


    }
}
