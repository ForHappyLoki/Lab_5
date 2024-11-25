using CourceWork.Data;
using CourceWork.Models;
using CourceWork.Services;
using CourceWork.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;

namespace CourceWork.Controllers
{
    [Authorize(Roles = "admin,moder")]
    public class GuestsController : Controller
    {
        private readonly DatabaseContext _db;
        private readonly GuestsService _guestsService;
        public GuestsController(DatabaseContext databaseContext, GuestsService guestsService)
        {
            _db = databaseContext;
            _guestsService = guestsService;

        }
        public async Task<IActionResult> Index(string searchTerm = null)
        {
            var model = await _guestsService.GetGuests();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                model = model.Where(m => m.FullName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> EditGuest(int guestId)
        {
            Guest guest;
            if (guestId > 0)
            {
                TempData["guestId"] = guestId;
                var guests = await _guestsService.GetGuests();
                guest = guests.FirstOrDefault(g => g.GuestId == guestId);
            }
            else
            {
                guest = new Guest();
            }
            return View(guest);
        }
        public async Task<IActionResult> ApprovalEditing(Guest guest)
        {
            if (TempData["guestId"] != null && int.TryParse(TempData["guestId"].ToString(), out int guestId))
            {
                guest.GuestId = guestId;
            }
            TempData.Remove("guestId");
            await _guestsService.Editing(guest);
            return RedirectToAction("Index", new { searchTerm = guest.FullName });
        }
        public async Task<IActionResult> Delete(int guestId)
        {
            await _guestsService.Delete(guestId);
            return RedirectToAction("Index");
        }
    }   
}
