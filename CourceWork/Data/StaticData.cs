﻿using CourceWork.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourceWork.Data
{
    [NotMapped]
    public static class StaticData
    {
        public static DatabaseContext StaticDB { get; set; }
        public static UserService StaticCacheUserService { get; set; }
        public static List<SelectListItem> Roles { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "user", Text = "Пользователь" },
            new SelectListItem { Value = "moder", Text = "Модератор" },
            new SelectListItem { Value = "admin", Text = "Администратор" }
        };

    }
}
