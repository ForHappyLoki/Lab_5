using CourceWork.Data;
using CourceWork.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourceWork.Controllers
{
    public class TvshowController : Controller
    {
        private DatabaseContext _db;
        private TvshowServices _tvshowServices;
        public TvshowController()
        {
            _db = StaticData.StaticDB;
            _tvshowServices = StaticData.TvshowServices;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int tvshowId)
        {
            var model = await _tvshowServices.GetTvshowModels(tvshowId);
            return View(model);
        }
    }
}
