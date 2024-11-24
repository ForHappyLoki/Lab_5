using CourceWork.Data;
using CourceWork.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourceWork.Controllers
{
    public class TvshowController : Controller
    {
        private readonly DatabaseContext _db;
        private readonly TvshowServices _tvshowServices;
        public TvshowController(DatabaseContext databaseContext, TvshowServices tvshowServices)
        {
            _db = databaseContext;
            _tvshowServices = tvshowServices;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int tvshowId)
        {
            var model = await _tvshowServices.GetTvshowModels(tvshowId);
            return View(model);
        }
    }
}
