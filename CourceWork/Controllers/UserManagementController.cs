using CourceWork.Data;
using CourceWork.Models;
using CourceWork.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace CourceWork.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly DatabaseContext _db;
        private readonly UserService _userService;
        public UserManagementController(DatabaseContext databaseContext, UserService userService)
        {
            _db = databaseContext;
            _userService = userService;
        }
        [HttpGet]
        public async Task<ActionResult> Index(string searchTerm, string position)
        {
            ViewBag.ShowNotification = "true";
            IEnumerable<Employee> model;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                model = await _userService.GetUsers(); 
                TempData.Remove("SearchTerm");
                ViewData.Remove("SearchTerm");
            }
            else
            {
                model = await _userService.GetUser(searchTerm);
                TempData["SearchTerm"] = searchTerm;
                ViewData["SearchTerm"] = searchTerm;
            }
            if(!string.IsNullOrWhiteSpace(position))
            {
                ViewData["SelectedPosition"] = position;
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(int employeeId)
        {
            await _userService.Delete(employeeId);
            var search = TempData["SearchTerm"] as string ?? "";
            return RedirectToAction("Index", new { searchTerm = search });
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> Editing(Employee model)
        {
            return View(model);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> ValidationEditing(Employee model)
        {
            if (ModelState.IsValid)
            {
                // Логика для обновления данных сотрудника в базе данных
                await _userService.UpdateUser(model);

                return RedirectToAction("Index", new { searchTerm = model.FullName });
            }
            return RedirectToAction("Editing", model);
        }
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Creation(Employee model)
        {
            Employee employee = new Employee();
            return View(employee);
        }
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> CreationEditing(Employee model)
        {
            if (ModelState.IsValid)
            {
                await _userService.AddUser(model);

                return RedirectToAction("Index", new { searchTerm = model.FullName });
            }

            return View();
        }
    }
}
