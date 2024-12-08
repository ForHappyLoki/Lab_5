using CourceWork.Data;
using CourceWork.Services;
using CourceWork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourceWork.Controllers
{
    public class CitizenAppealController(DatabaseContext context, CitizenAppealService citizenAppealService) : Controller
    {
        DatabaseContext db = context;
        CitizenAppealService citizenAppealService = citizenAppealService;
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            CitizenAppealModel model = new CitizenAppealModel();
            model.AllShows = await citizenAppealService.GetShows();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ApprovalAppeal(CitizenAppealModel model)
        {
            await citizenAppealService.SaveCitizenAppeal(model);
            return View();
        }
        [Authorize(Roles = "moder,admin")]
        public async Task<IActionResult> AdminIndex()
        {
            List<CitizenAppealModel> model = await citizenAppealService.GetModel();
            return View(model);
        }

        [Authorize(Roles = "moder,admin")]
        public async Task<IActionResult> DeleteAppeal(int appealId)
        {
            await citizenAppealService.DeleteAppeal(appealId);
            return RedirectToAction("AdminIndex");
        }
    }
}
