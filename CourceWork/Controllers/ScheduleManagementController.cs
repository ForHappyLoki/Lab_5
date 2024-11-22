﻿using CourceWork.Data;
using CourceWork.Models;
using CourceWork.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourceWork.Controllers
{
    public class ScheduleManagementController : Controller
    {
        private DatabaseContext _db;
        private ScheduleService scheduleService;
        public ScheduleManagementController()
        {
            _db = StaticData.StaticDB;
            scheduleService = StaticData.StaticScheduleService;
        }
        public async Task<IActionResult> Index(DateTime date)
        {
            var model = await scheduleService.ChooseDay(date);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddShowToSchedule(int scheduleId, int showId, int order, DateTime date)
        {
            await scheduleService.AddShowToSchedule(scheduleId, showId, order, date);
            return RedirectToAction("Index", new { date });
        }
        [HttpPost]
        public async Task<IActionResult> EditShowToSchedule(List<ScheduleTvshow> scheduleTvshows, DateTime date)
        {
            await scheduleService.EditShowToSchedule(scheduleTvshows, date);
            return RedirectToAction("Index", new { date });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteShowToSchedule(int scheduleTvshowId)
        {
            DateOnly date = await scheduleService.DeleteShowToSchedule(scheduleTvshowId);
            return RedirectToAction("Index", new DateTime(date.Year, date.Month, date.Day));
        }
    }
}
