using CourceWork.Data;
using CourceWork.Models;
using CourceWork.Services;
using CourceWork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;

namespace CourceWork.Controllers
{
    [Authorize(Roles = "admin,moder")]
    public class CreateShowController : Controller
    {
        private DatabaseContext _db;
        private TvshowServices _tvshowServices;
        public CreateShowController()
        {
            _db = StaticData.StaticDB;
            _tvshowServices = StaticData.TvshowServices;
        }
        public async Task<IActionResult> Index(string searchTerm)
        {
            StaticData.modelCache = new TvshowModel();
            List<TvshowModel> tvshowModel;
            if (string.IsNullOrEmpty(searchTerm))
            {
                tvshowModel = await _tvshowServices.GetTvshowModels();
            }
            else
            {
                tvshowModel = await _tvshowServices.GetTvshowModels(searchTerm);
            }
            ViewData["SearchTerm"] = searchTerm;
            return View(tvshowModel);
        }
        [HttpPost]
        public async Task<IActionResult> ShowCreation(TvshowModel model, string action = null, int modelId = 0)
        {
            if (modelId > 0)
            {
                TempData["modelId"] = modelId;
                List<TvshowModel> tvshowModel = await _tvshowServices.GetTvshowModels();
                model = tvshowModel.FirstOrDefault(t => t.tvshow.ShowId == modelId);
                foreach(var employee in model.employee)
                {
                    StaticData.modelCache.employee.Add(employee);
                }
                foreach(var guest in model.guest)
                {
                    StaticData.modelCache.guest.Add(guest);
                }
            }    
            if (string.IsNullOrEmpty(action))
            {
                if (model == null)
                {
                    model = new TvshowModel();
                }
                return View(model);
            }
            switch (action)
            {
                case "AddEmployee":
                    // Логика добавления сотрудника
                    var selectedEmployee = StaticData.allEmployeesModel.employee
                        .FirstOrDefault(e => e.EmployeeId == model.SelectedEmployeeId);
                    var dublicate = StaticData.modelCache.employee.Any(d => d.EmployeeId == selectedEmployee.EmployeeId);
                    if (selectedEmployee != null && !dublicate)
                    {
                        TvshowEmployee tvshowEmployee = new TvshowEmployee()
                        {
                            ShowId = model.tvshow.ShowId,
                            EmployeeId = selectedEmployee.EmployeeId,
                        };
                        StaticData.modelCache.tvshowEmployeeNew.Add(tvshowEmployee);
                        StaticData.modelCache.employee.Add(selectedEmployee);
                    }
                    model.SelectedEmployeeId = 0;
                    break;
                case "DeleteEmployee":
                    // Логика удаления сотрудника
                    selectedEmployee = StaticData.allEmployeesModel.employee
                        .FirstOrDefault(e => e.EmployeeId == model.SelectedEmployeeId);
                    if (selectedEmployee != null)
                    {
                        var tvshowEmployee = StaticData.modelCache.tvshowEmployeeNew
                            .FirstOrDefault(t => t.EmployeeId == selectedEmployee.EmployeeId);
                        StaticData.modelCache.tvshowEmployeeNew.Remove(tvshowEmployee);
                        StaticData.modelCache.employee.Remove(selectedEmployee);
                    }
                    model.SelectedEmployeeId = 0;
                    break;

                case "AddGuest":
                    // Логика добавления гостя
                    var selectedGuest = StaticData.allGuestsModel.guests
                        .FirstOrDefault(g => g.GuestId == model.SelectedGuestId);
                    dublicate = StaticData.modelCache.guest.Any(d => d.GuestId == selectedGuest.GuestId);
                    if (selectedGuest != null && !dublicate)
                    {
                        TvshowGuest tvshowGuest = new TvshowGuest()
                        {
                            ShowId = model.tvshow.ShowId,
                            GuestId = selectedGuest.GuestId,
                        };
                        StaticData.modelCache.tvshowGuestsNew.Add(tvshowGuest);
                        StaticData.modelCache.guest.Add(selectedGuest);
                    }
                    model.SelectedGuestId = 0;
                    break;

                case "DeleteGuest":
                    // Логика добавления гостя
                    selectedGuest = StaticData.allGuestsModel.guests
                        .FirstOrDefault(g => g.GuestId == model.SelectedGuestId);
                    if (selectedGuest != null)
                    {
                        TvshowGuest tvshowGuest = StaticData.modelCache.tvshowGuest
                            .FirstOrDefault(t => t.GuestId == selectedGuest.GuestId);
                        StaticData.modelCache.tvshowGuestsNew.Remove(tvshowGuest);
                        StaticData.modelCache.guest.Remove(selectedGuest);
                    }
                    model.SelectedGuestId = 0;
                    break;
                case "Save":
                    model.guest = StaticData.modelCache.guest;
                    model.employee = StaticData.modelCache.employee;
                    StaticData.modelCache = new TvshowModel();
                    // Финальное сохранение в БД
                    int showId = 0;
                    if (TempData.TryGetValue("modelId", out var modelIdValue) && modelIdValue is int id)
                    {
                        showId = id;
                        TempData.Remove("modelId");
                    }
                    await _tvshowServices.SaveShow(model, showId);
                    return RedirectToAction("Index", new { searchTerm = model.tvshow.Title });
            }
            StaticData.allEmployeesModel = await _tvshowServices.GetAllEmployeesModel();
            StaticData.allGenresModel = await _tvshowServices.GetAllGenresModel();
            StaticData.allGuestsModel = await _tvshowServices.GetAllGuestsModel();
            if (modelId == 0)
            {
                model.guest = StaticData.modelCache.guest;
                model.employee = StaticData.modelCache.employee;
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ShowDelete(int showId)
        {
            await _tvshowServices.DeleteShow(showId);
            return RedirectToAction("Index");
        }
    }
}
