using ALJEproject.Data;
using ALJEproject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;
using System.IO;
using ALJEproject.Services.Interfaces;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ALJEproject.Controllers
{
    public class MenuController : Controller
    {
        private readonly ILogger<MenuController> _logger;
        private readonly ALJEprojectDbContext _context;
        private readonly IUserService _userService;

        // Constructor that combines both dependencies
        public MenuController(IUserService userService, ALJEprojectDbContext context, ILogger<MenuController> logger)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
        }

        // GET: Menu
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {

            IEnumerable<Menu> menus;
            int totalMenus;

            if (!string.IsNullOrWhiteSpace(search))
            {
                menus = _userService.SearchMenus(search, page, pageSize);
                totalMenus = _userService.GetTotalMenusCount(search);
            }
            else
            {
                menus = _userService.GetPaginatedMenus(page, pageSize);
                totalMenus = _userService.GetTotalMenusCount();
            }

            var model = new PaginatedMenuViewModel
            {
                Menus = menus,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalMenus
            };

            ViewData["CurrentSearch"] = search;

            return View(model);
        }



        // GET: Menu/Create
        [HttpGet]
        public IActionResult Create()
        {
            var parentMenus = _context.Menus
            .Where(m => m.Active && m.ParentMenuID==null) // Hanya ambil menu yang aktif (opsional)
            .Select(r => new ParentMenuItem { MenuID = r.MenuID, MenuDesc = r.MenuName }) // Map ke model khusus
            .ToList();

            // Tambahkan opsi 'Parent' dengan nilai NULL
            parentMenus.Insert(0, new ParentMenuItem { MenuID = null, MenuDesc = "Set Parent" });

            ViewBag.ParentMenus = new SelectList(parentMenus, "MenuID", "MenuDesc");

            return PartialView("_CreateMenuPartial");
        }

        // POST: Menu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Menu menu)
        {

            if (ModelState.IsValid)
            {
                string createdBy = HttpContext.Session.GetString("Username");
                menu.CreatedDate = DateTime.Now;
                menu.CreatedBy = createdBy; // Set as appropriate, e.g., logged-in user
                _context.Menus.Add(menu);
                _context.SaveChanges();
                return Json(new { success = true });
            }

            var parentMenus = _context.Menus.Select(r => new { r.ParentMenuID, r.MenuName }).ToList();
            ViewBag.ParentMenus = new SelectList(parentMenus, "MenuID", "MenuDesc");

            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList() });
        }

        // GET: Menu/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var menu = _context.Menus.Find(id);
            if (menu == null)
            {
                return NotFound();
            }

            return PartialView("_EditMenuPartial", menu);
        }

        // POST: Menu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Menu menu)
        {
            if (ModelState.IsValid)
            {
                string updateBy = HttpContext.Session.GetString("Username");
                try
                {
                    menu.UpdatedDate = DateTime.Now;
                    menu.UpdatedBy = updateBy; // Set as appropriate, e.g., logged-in user
                    _context.Update(menu);
                    _context.SaveChanges();
                    _logger.LogInformation("Menu with ID {MenuId} updated successfully.", menu.MenuID);
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating menu with ID {MenuId}.", menu.MenuID);
                    return Json(new { success = false, message = "An error occurred while updating the menu." });
                }
            }

            var errorMessages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            _logger.LogError("Failed to update menu with ID {MenuId}. Model state is invalid.", menu.MenuID);

            return Json(new { success = false, errors = errorMessages });
        }

        // POST: Menu/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var menu = _context.Menus.Find(id);
            if (menu != null)
            {
                _context.Menus.Remove(menu);
                _context.SaveChanges();
                _logger.LogInformation("Menu with ID {MenuId} deleted successfully.", id);
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = new[] { "Menu not found." } });
        }

        public IActionResult View(int id)
        {
            var menu = _context.Menus.FirstOrDefault(u => u.MenuID == id);
            if (menu == null)
            {
                return NotFound();
            }
            return PartialView("_ViewMenuPartial", menu); // Use a partial view for the modal content
        }

        public IActionResult ExportToExcel()
        {
            // Assuming you have a method to get option data
            var menus = _context.Menus.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("menus");
                worksheet.Cell(1, 1).Value = "MenuID";
                worksheet.Cell(1, 2).Value = "MenuName";
                worksheet.Cell(1, 3).Value = "MenuURL";
                worksheet.Cell(1, 4).Value = "ControllerName";
                worksheet.Cell(1, 5).Value = "CreatedBy";
                worksheet.Cell(1, 6).Value = "UpdatedBy";

                for (int i = 0; i < menus.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = menus[i].MenuID;
                    worksheet.Cell(i + 2, 2).Value = menus[i].MenuName;
                    worksheet.Cell(i + 2, 3).Value = menus[i].MenuURL;
                    worksheet.Cell(i + 2, 4).Value = menus[i].ControllerName;
                    worksheet.Cell(i + 2, 5).Value = menus[i].CreatedBy;
                    worksheet.Cell(i + 2, 6).Value = menus[i].UpdatedBy;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ListMenu.xlsx");
                }
            }
        }
    }
}
