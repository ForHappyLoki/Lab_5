using CourceWork.Services;
using CourceWork.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourceWork.Data
{
    [NotMapped]
    public static class StaticData
    {
        public static DatabaseContext StaticDB { get; set; }
        public static UserService StaticCacheUserService { get; set; }
        public static ScheduleService StaticScheduleService { get; set; }
        public static TvshowServices TvshowServices { get; set; }
        public static List<SelectListItem> Roles { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "user", Text = "Пользователь" },
            new SelectListItem { Value = "moder", Text = "Модератор" },
            new SelectListItem { Value = "admin", Text = "Администратор" }
        };
        public static AllEmployeesModel allEmployeesModel;
        public static AllGenresModel allGenresModel;
        public static AllGuestsModel allGuestsModel;
        public static AllTvshowEmployeesModel allTvshowEmployeesModel;
        public static AllTvshowGuestsModel allTvshowGuestsModel;

        public static TvshowModel modelCache = new TvshowModel();

    }
}
