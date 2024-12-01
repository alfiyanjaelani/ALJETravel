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
using ALJEproject.Services.Interfaces;
using System.Collections.Generic;
using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace ALJEproject.Controllers
{
    public class UserAccessController : Controller
    {
        private readonly ILogger<UserAccessController> _logger;
        private readonly ALJEprojectDbContext _context;
        private readonly IUserService _userService;

        // Constructor to combine both dependencies
        public UserAccessController(IUserService userService, ALJEprojectDbContext context, ILogger<UserAccessController> logger)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
        }

        // GET: UserAccess
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var menus = await _userService.GetActiveMenusAsync();
            ViewBag.Menus = menus;
            IEnumerable<UserAccessView> userAccess;
            int totalUserAccesses; // Declare a variable for total user access count

            if (!string.IsNullOrWhiteSpace(search))
            {
                userAccess = _userService.SearchUserAccesses(search, page, pageSize);
                // Get the total count of user accesses matching the search criteria
                totalUserAccesses = _userService.GetTotalUserAccessesCount(search); // Pass the search term to count matching user accesses
            }
            else
            {
                userAccess = _userService.GetPaginatedUserAccesses(page, pageSize);
                totalUserAccesses = _userService.GetTotalUserAccessesCount(); // Get the total user access count without filtering
            }

            var model = new PaginatedUserAccesViewModel
            {
                userAccess = userAccess,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalUserAccesses // Reflects the correct count based on search
            };

            ViewData["CurrentSearch"] = search; // Save search term to display in the view

            return View(model);
        }

        // GET: UserAccess/Create
        [HttpGet]
        public IActionResult Create()
        {
            var roles = _context.Roles.Select(r => new { r.RoleID, r.RoleName }).ToList();
            ViewBag.RoleList = new SelectList(roles, "RoleID", "RoleName");

            var menus = _context.Menus.Select(m => new { m.MenuID, m.MenuName }).ToList();
            ViewBag.MenuList = new SelectList(menus, "MenuID", "MenuName");

            return PartialView("_CreateUserAccessPartial");
        }

        // POST: UserAccess/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserAccess userAccess)
        {
            if (ModelState.IsValid)
            {
                string createdBy = HttpContext.Session.GetString("Username");
                userAccess.CreatedDate = DateTime.Now;
                userAccess.CreatedBy = createdBy; // Assuming you want to use the current user's name
                _context.UserAccesses.Add(userAccess);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
        }

        // GET: UserAccess/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var userAccess = _context.UserAccesses.Find(id);
            if (userAccess == null)
            {
                return NotFound();
            }

            var roles = _context.Roles.Select(r => new { r.RoleID, r.RoleName }).ToList();
            ViewBag.RoleList = new SelectList(roles, "RoleID", "RoleName", userAccess.RoleID);

            var menus = _context.Menus.Select(m => new { m.MenuID, m.MenuName }).ToList();
            ViewBag.MenuList = new SelectList(menus, "MenuID", "MenuName", userAccess.MenuID);

            return PartialView("_EditUserAccessPartial", userAccess);
        }

        // POST: UserAccess/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserAccess userAccess)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string updateBy = HttpContext.Session.GetString("Username");
                    userAccess.UpdatedDate = DateTime.Now;
                    userAccess.UpdatedBy = updateBy; // Assuming you want to use the current user's name
                    _context.Update(userAccess);
                    _context.SaveChanges();
                    _logger.LogInformation("UserAccess with ID {UserAccessId} updated successfully.", userAccess.UserAccessID);
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating UserAccess with ID {UserAccessId}.", userAccess.UserAccessID);
                    return Json(new { success = false, message = "An error occurred while updating the user access." });
                }
            }

            var errorMessages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            _logger.LogError("Failed to update UserAccess with ID {UserAccessId}. Model state is invalid.", userAccess.UserAccessID);

            // If the model state is not valid, repopulate the dropdowns
            var roles = _context.Roles.Select(r => new { r.RoleID, r.RoleName }).ToList();
            ViewBag.RoleList = new SelectList(roles, "RoleID", "RoleName", userAccess.RoleID);

            var menus = _context.Menus.Select(m => new { m.MenuID, m.MenuName }).ToList();
            ViewBag.MenuList = new SelectList(menus, "MenuID", "MenuName", userAccess.MenuID);

            return Json(new { success = false, errors = errorMessages });
        }

        // POST: UserAccess/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var userAccess = _context.UserAccesses.Find(id);
            if (userAccess != null)
            {
                _context.UserAccesses.Remove(userAccess);
                _context.SaveChanges();
                _logger.LogInformation("UserAccess with ID {UserAccessId} deleted successfully.", id);
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = new[] { "UserAccess not found." } });
        }

        public IActionResult ExportToExcel()
        {
            // Assuming you have a method to get user data
            var users = _context.UserAccessesView.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Users");
                worksheet.Cell(1, 1).Value = "UserAccessID";
                worksheet.Cell(1, 2).Value = "RoleName";
                worksheet.Cell(1, 3).Value = "MenuName";           

                for (int i = 0; i < users.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = users[i].UserAccessID;
                    worksheet.Cell(i + 2, 2).Value = users[i].RoleName;
                    worksheet.Cell(i + 2, 3).Value = users[i].MenuName;                  
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UsersAccess.xlsx");
                }
            }
        }
    }
}
