using ALJEproject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using ALJEproject.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using ALJEproject.Services.Interfaces;
using System.Collections.Generic;
using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace ALJEproject.Controllers
{
    public class RoleController : Controller
    {
        private readonly ILogger<RoleController> _logger;
        private readonly ALJEprojectDbContext _context;
        private readonly IUserService _userService;

        // Konstruktor yang menggabungkan kedua dependensi
        public RoleController(IUserService userService, ALJEprojectDbContext context, ILogger<RoleController> logger)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
        }

        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var menus = await _userService.GetActiveMenusAsync();
            ViewBag.Menus = menus;
            IEnumerable<RoleView> roles; // Assuming you have a RoleViewModel
            int totalRoles; // Declare a variable for total role count

            if (!string.IsNullOrWhiteSpace(search))
            {
                roles = _userService.SearchRoles(search, page, pageSize);
                // Get the total count of roles matching the search criteria
                totalRoles = _userService.GetTotalRolesCount(search); // Pass the search term to count matching roles
            }
            else
            {
                roles = _userService.GetPaginatedRoles(page, pageSize);
                totalRoles = _userService.GetTotalRolesCount(); // Get the total role count without filtering
            }

            var model = new PaginatedRoleViewModel
            {
                Roles = roles,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalRoles // This will now reflect the correct count based on search
            };

            ViewData["CurrentSearch"] = search; // Save search term to display in the view

            return View(model);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreateRolePartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Role role)
        {
            if (ModelState.IsValid)
            {
                string createdBy = HttpContext.Session.GetString("Username");
                role.CreatedDate = DateTime.Now;
                role.CreatedBy = createdBy; // Atur CreatedBy dari user yang sedang login
                _context.Roles.Add(role);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
        }

        // GET: Role/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var role = _context.Roles.Find(id);
            if (role == null)
            {
                return NotFound();
            }
            return PartialView("_EditRolePartial", role);
        }

        // POST: Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Role role)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string updateBy = HttpContext.Session.GetString("Username");
                    role.UpdatedDate = DateTime.Now;
                    role.UpdatedBy = updateBy; // Atur UpdatedBy dari user yang sedang login
                    _context.Update(role);
                    _context.SaveChanges();
                    _logger.LogInformation("Role with ID {RoleId} updated successfully.", role.RoleID);
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating role with ID {RoleId}.", role.RoleID);
                    return Json(new { success = false, message = "An error occurred while updating the role." });
                }
            }

            var errorMessages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            _logger.LogError("Failed to update role with ID {RoleId}. Model state is invalid.", role.RoleID);

            return Json(new { success = false, errors = errorMessages });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var role = _context.Roles.Find(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                _context.SaveChanges();
                _logger.LogInformation("Role with ID {RoleId} deleted successfully.", id);
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = new[] { "Role not found." } });
        }

        public IActionResult View(int id)
        {
            var role = _context.Roles.FirstOrDefault(u => u.RoleID == id);
            if (role == null)
            {
                return NotFound();
            }
            return PartialView("_ViewRolePartial", role); // Use a partial view for the modal content
        }

        public IActionResult ExportToExcel()
        {
            // Assuming you have a method to get option data
            var roles = _context.Roles.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("roles");
                worksheet.Cell(1, 1).Value = "RoleID";
                worksheet.Cell(1, 2).Value = "RoleName";
                worksheet.Cell(1, 3).Value = "CreatedBy";
                worksheet.Cell(1, 4).Value = "CreatedDate";
                worksheet.Cell(1, 5).Value = "UpdatedDate";
                worksheet.Cell(1, 6).Value = "UpdatedBy";

                for (int i = 0; i < roles.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = roles[i].RoleID;
                    worksheet.Cell(i + 2, 2).Value = roles[i].RoleName;
                    worksheet.Cell(i + 2, 3).Value = roles[i].CreatedBy;
                    worksheet.Cell(i + 2, 4).Value = roles[i].CreatedDate;
                    worksheet.Cell(i + 2, 5).Value = roles[i].UpdatedDate;
                    worksheet.Cell(i + 2, 6).Value = roles[i].UpdatedBy;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ListRole.xlsx");
                }
            }
        }

    }
}
