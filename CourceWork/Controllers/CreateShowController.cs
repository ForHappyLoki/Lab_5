﻿using CourceWork.Data;
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
        private readonly DatabaseContext _db;
        private readonly TvshowServices _tvshowServices;
        private TvshowModel tvshowModelCache;
        public CreateShowController(DatabaseContext databaseContext, TvshowServices tvshowServices)
        {
            _db = databaseContext;
            _tvshowServices = tvshowServices;

        }
        public async Task<IActionResult> Index(string searchTerm)
        {
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
            tvshowModelCache = _tvshowServices.CacheTvshowModelResave();
            if (modelId > 0)
            {
                TempData["modelId"] = modelId;
                List<TvshowModel> tvshowModel = await _tvshowServices.GetTvshowModels();
                model = tvshowModel.FirstOrDefault(t => t.tvshow.ShowId == modelId);
                _tvshowServices.AddObjects(model.employee);
                _tvshowServices.AddObjects(model.guest);
            }    
            if (string.IsNullOrEmpty(action))
            {
                if (model == null)
                {
                    model = tvshowModelCache;
                }
                return View(model);
            }
            switch (action)
            {
                case "AddEmployee":
                    // Логика добавления сотрудника
                    var selectedEmployee = TvshowModel.allEmployeesModel.employee
                        .FirstOrDefault(e => e.EmployeeId == model.SelectedEmployeeId);
                    var dublicate = _tvshowServices.CacheTvshowModelResave().employee
                        .Any(d => d.EmployeeId == selectedEmployee.EmployeeId);
                    if (selectedEmployee != null && !dublicate)
                    {
                        _tvshowServices.AddObject(selectedEmployee);
                    }
                    model.SelectedEmployeeId = 0;
                    break;
                case "DeleteEmployee":
                    // Логика удаления сотрудника
                    selectedEmployee = TvshowModel.allEmployeesModel.employee
                        .FirstOrDefault(e => e.EmployeeId == model.SelectedEmployeeId);
                    if (selectedEmployee != null)
                    {
                        var tvshowEmployee = _tvshowServices.CacheTvshowModelResave().tvshowEmployeeNew
                            .FirstOrDefault(t => t.EmployeeId == selectedEmployee.EmployeeId);
                        _tvshowServices.RemoveObject(selectedEmployee);
                    }
                    model.SelectedEmployeeId = 0;
                    break;

                case "AddGuest":
                    // Логика добавления гостя
                    var selectedGuest = TvshowModel.allGuestsModel.guests
                        .FirstOrDefault(g => g.GuestId == model.SelectedGuestId);
                    dublicate = _tvshowServices.CacheTvshowModelResave().guest.Any(d => d.GuestId == selectedGuest.GuestId);
                    if (selectedGuest != null && !dublicate)
                    {
                        _tvshowServices.AddObject(selectedGuest);
                    }
                    model.SelectedGuestId = 0;
                    break;

                case "DeleteGuest":
                    // Логика добавления гостя
                    selectedGuest = TvshowModel.allGuestsModel.guests
                        .FirstOrDefault(g => g.GuestId == model.SelectedGuestId);
                    if (selectedGuest != null)
                    {
                        TvshowGuest tvshowGuest = _tvshowServices.CacheTvshowModelResave().tvshowGuest
                            .FirstOrDefault(t => t.GuestId == selectedGuest.GuestId);
                        _tvshowServices.RemoveObject(selectedGuest);
                    }
                    model.SelectedGuestId = 0;
                    break;
                case "Save":
                    model.guest = _tvshowServices.CacheTvshowModelResave().guest;
                    model.employee = _tvshowServices.CacheTvshowModelResave().employee;
                    _tvshowServices.RenewTvCache();
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
            TvshowModel.allEmployeesModel = await _tvshowServices.GetAllEmployeesModel();
            TvshowModel.allGenresModel = await _tvshowServices.GetAllGenresModel();
            TvshowModel.allGuestsModel = await _tvshowServices.GetAllGuestsModel();
            if (modelId == 0)
            {
                model.guest = _tvshowServices.CacheTvshowModelResave().guest;
                model.employee = _tvshowServices.CacheTvshowModelResave().employee;
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
