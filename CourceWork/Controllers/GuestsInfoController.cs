using CourceWork.Data;
using CourceWork.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourceWork.Controllers
{
    public class GuestsInfoController(DatabaseContext databaseContext, GuestsService guestsService)
        : Controller
    {
        private readonly DatabaseContext _db = databaseContext;
        private readonly GuestsService _guestsService = guestsService;
        public async Task<IActionResult> Index(int guestId)
        {
            var model = await _guestsService.GetGuest(guestId);
            return View(model);
        }
    }
}
