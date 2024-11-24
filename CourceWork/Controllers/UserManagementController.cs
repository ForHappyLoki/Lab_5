﻿using CourceWork.Data;
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
    [Authorize(Roles = "admin")]
    public class UserManagementController : Controller
    {
        private readonly DatabaseContext _db;
        private readonly UserService _userService;
        public UserManagementController(DatabaseContext databaseContext, UserService userService)
        {
            _db = databaseContext;
            _userService = userService;
        }
        public async Task<ActionResult> Index(string searchTerm)
        {
            ViewBag.ShowNotification = "true";
            IEnumerable<Employee> model;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                model = await _userService.GetUsers(); 
            }
            else
            {
                model = await _userService.GetUser(searchTerm); 
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int employeeId)
        {
            await _userService.Delete(employeeId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<ActionResult> Editing(Employee model)
        {
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> ValidationEditing(Employee model)
        {
            if (ModelState.IsValid)
            {
                // Логика для обновления данных сотрудника в базе данных
                await _userService.UpdateUser(model);

                return RedirectToAction("Index");
            }
            return RedirectToAction("Editing", model);
        }
        public async Task<ActionResult> Creation(Employee model)
        {
            Employee employee = new Employee();
            return View(employee);
        }
        public async Task<ActionResult> CreationEditing(Employee model)
        {
            if (ModelState.IsValid)
            {
                await _userService.AddUser(model);

                return RedirectToAction("Index");
            }

            return View();
        }
    }
}
