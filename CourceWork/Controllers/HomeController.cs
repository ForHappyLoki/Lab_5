using CourceWork.Data;
using CourceWork.Models;
using CourceWork.Services;
using CourceWork.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CourceWork.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private DatabaseContext _db;
        private ScheduleService scheduleService;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _db = StaticData.StaticDB;
            scheduleService = StaticData.StaticScheduleService;
        }

        public async Task<IActionResult> Index(DateOnly? date = null)
        {
            DateOnly dateOnly = date ?? DateOnly.FromDateTime(DateTime.Now);
            ScheduleModel model = await scheduleService.ChooseWeek(dateOnly);
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
