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
    public class OptionController : Controller
    {
        private readonly ILogger<OptionController> _logger;
        private readonly ALJEprojectDbContext _context;
        private readonly IUserService _userService;

        // Constructor for dependency injection
        public OptionController(IUserService userService, ALJEprojectDbContext context, ILogger<OptionController> logger)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
        }

        // Display the list of roles (or options)
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var menus = await _userService.GetActiveMenusAsync();
            ViewBag.Menus = menus;
            IEnumerable<Option> options; // Assuming you have an OptionView model
            int totalOptions; // Declare a variable for total option count

            if (!string.IsNullOrWhiteSpace(search))
            {
                options = _userService.SearchOptions(search, page, pageSize);
                // Get the total count of options matching the search criteria
                totalOptions = _userService.GetTotalOptionsCount(search); // Pass the search term to count matching options
            }
            else
            {
                options = _userService.GetPaginatedOptions(page, pageSize);
                totalOptions = _userService.GetTotalOptionsCount(); // Get the total option count without filtering
            }

            var model = new PaginatedOptionViewModel
            {
                Options = options,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalOptions // This will now reflect the correct count based on search
            };

            ViewData["CurrentSearch"] = search; // Save search term to display in the view

            return View(model);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreateOptionPartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Option option)
        {
            if (ModelState.IsValid)
            {
                string createdBy = HttpContext.Session.GetString("Username");

                option.CreatedDate = DateTime.Now;
                option.CreatedBy = createdBy; // Set CreatedBy from logged-in user
                _context.Options.Add(option);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
        }

        // GET: Option/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var option = _context.Options.Find(id);
            if (option == null)
            {
                return NotFound();
            }
            return PartialView("_EditOptionPartial", option);
        }

        // POST: Option/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Option option)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string updateBy = HttpContext.Session.GetString("Username");

                    option.UpdatedDate = DateTime.Now;
                    option.UpdatedBy = updateBy; // Set UpdatedBy from logged-in user
                    _context.Update(option);
                    _context.SaveChanges();
                    _logger.LogInformation("Option with ID {optionId} updated successfully.", option.OptionsID);
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating option with ID {optionId}.", option.OptionsID);
                    return Json(new { success = false, message = "An error occurred while updating the option." });
                }
            }

            var errorMessages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            _logger.LogError("Failed to update option with ID {optionId}. Model state is invalid.", option.OptionsID);

            return Json(new { success = false, errors = errorMessages });
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var option = _context.Options.Find(id);
            if (option != null)
            {
                _context.Options.Remove(option);
                _context.SaveChanges();
                _logger.LogInformation("Option with ID {OptionId} deleted successfully.", id);
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = new[] { "Option not found." } });
        }
        public IActionResult View(int id)
        {
            var option = _context.Options.FirstOrDefault(u => u.OptionsID == id);
            if (option == null)
            {
                return NotFound();
            }
            return PartialView("_ViewOptionPartial", option); // Use a partial view for the modal content
        }

        public IActionResult ExportToExcel()
        {
            // Assuming you have a method to get option data
            var options = _context.Options.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Options");
                worksheet.Cell(1, 1).Value = "OptionID";
                worksheet.Cell(1, 2).Value = "FieldName";
                worksheet.Cell(1, 3).Value = "FieldValue";
                worksheet.Cell(1, 4).Value = "LongName";
                worksheet.Cell(1, 5).Value = "CreatedBy";
                worksheet.Cell(1, 6).Value = "UpdatedBy";

                for (int i = 0; i < options.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = options[i].OptionsID;
                    worksheet.Cell(i + 2, 2).Value = options[i].FieldName;
                    worksheet.Cell(i + 2, 3).Value = options[i].FieldValue;
                    worksheet.Cell(i + 2, 4).Value = options[i].LongName;
                    worksheet.Cell(i + 2, 5).Value = options[i].CreatedBy;
                    worksheet.Cell(i + 2, 6).Value = options[i].UpdatedBy;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ListOption.xlsx");
                }
            }
        }

    }
}
