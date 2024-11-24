using CourceWork.Data;
using CourceWork.Models;
using CourceWork.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CourceWork.Services
{
    public class TvshowServices
    {
        private DatabaseContext db;
        public IMemoryCache cache;
        public TvshowServices(DatabaseContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }
        public async Task<List<TvshowModel>> GetTvshowModels()
        {
            List<TvshowModel> tvshowModels = null;
            if (!cache.TryGetValue($"TvshowModel", out tvshowModels))
            {
                tvshowModels = new List<TvshowModel>();

                var tvshowsTask = await db.Tvshows.ToListAsync();
                var genresTask = await db.Genres.ToListAsync();
                var tvshowEmployeesTask = await db.TvshowEmployees.ToListAsync();
                var employeesTask = await db.Employees.ToListAsync();
                var tvshowGuestsTask = await db.TvshowGuests.ToListAsync();
                var guestsTask = await db.Guests.ToListAsync();

                var tvshows = tvshowsTask;
                var genres = genresTask;
                var tvshowEmployees = tvshowEmployeesTask;
                var employees = employeesTask;
                var tvshowGuests = tvshowGuestsTask;
                var guests = guestsTask;
                foreach (var tvshow in tvshows)
                {
                    var genre = genres.FirstOrDefault(g => g.GenreId == tvshow.GenreId);
                    var tvshowEmployee = tvshowEmployees.Where(t => t.ShowId == tvshow.ShowId);
                    // Получаем список всех идентификаторов сотрудников из tvshowEmployee
                    var employeeIds = tvshowEmployee.Select(te => te.EmployeeId).ToList();
                    // Фильтруем список сотрудников, чтобы оставить только тех, чьи идентификаторы есть в employeeIds
                    var filteredEmployees = employees.Where(e => employeeIds.Contains(e.EmployeeId)).ToList();

                    var tvshowGuest = tvshowGuests.Where(t => t.ShowId == tvshow.ShowId);
                    var guestIds = tvshowGuest.Select(te => te.GuestId).ToList();
                    var filteredGuest = guests.Where(e => guestIds.Contains(e.GuestId)).ToList();
                    TvshowModel tvshowModel = new TvshowModel()
                    {
                        tvshow = tvshow,
                        genre = genre,
                        GenreId = genre.GenreId,
                        tvshowEmployee = tvshowEmployee.ToList(),
                        employee = filteredEmployees,
                        tvshowGuest = tvshowGuest.ToList(),
                        guest = filteredGuest.ToList()
                    };
                    tvshowModels.Add(tvshowModel);
                }
                tvshowModels = tvshowModels.OrderBy(t => t.tvshow.Title).ToList();
                if (tvshowModels != null)
                {
                    cache.Set($"TvshowModel", tvshowModels, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
            }
            return tvshowModels;
        }
        public async Task<List<TvshowModel>> GetTvshowModels(string searchTerm)
        {
            List<TvshowModel> tvshowModels = await GetTvshowModels();
            return tvshowModels = tvshowModels
                .Where(t => t.tvshow.Title.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }
        public async Task<TvshowModel> GetTvshowModels(int showId)
        {
            List<TvshowModel> tvshowModels = await GetTvshowModels();
            return tvshowModels.FirstOrDefault(t => t.tvshow.ShowId == showId);
        }
        public async Task<AllEmployeesModel> GetAllEmployeesModel()
        {
            AllEmployeesModel allEmplouee = null;
            if (!cache.TryGetValue($"AllEmployeesModel", out allEmplouee))
            {
                allEmplouee = new AllEmployeesModel();

                allEmplouee.employee = await db.Employees.ToListAsync();

                if (allEmplouee.employee != null)
                {
                    cache.Set($"AllEmployeesModel", allEmplouee, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
            }
            return allEmplouee;
        }
        public async Task<AllGenresModel> GetAllGenresModel()
        {
            AllGenresModel allGenres = null;

            if (!cache.TryGetValue($"AllGenresModel", out allGenres))
            {
                allGenres = new AllGenresModel();
                allGenres.genres = await db.Genres.ToListAsync();
                if (allGenres.genres != null)
                {
                    cache.Set($"AllGenresModel", allGenres, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
            }
            return allGenres;
        }
        public async Task<AllGuestsModel> GetAllGuestsModel()
        {
            AllGuestsModel allGuestsModel = null;

            if (!cache.TryGetValue($"AllGuestsModel", out allGuestsModel))
            {
                allGuestsModel= new AllGuestsModel();
                allGuestsModel.guests = await db.Guests.ToListAsync();
                if(allGuestsModel != null)
                {
                    cache.Set($"AllGuestsModel", allGuestsModel, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
            }
            return allGuestsModel;
        }
        public async Task<AllTvshowEmployeesModel> GetAllTvshowEmployeesModel()
        {
            AllTvshowEmployeesModel allTvshowEmployeesModel = null;

            if (!cache.TryGetValue($"AllTvshowEmployeesModel", out allTvshowEmployeesModel))
            {
                allTvshowEmployeesModel = new AllTvshowEmployeesModel();
                allTvshowEmployeesModel.tvshowEmployee = await db.TvshowEmployees.ToListAsync();
                if (allTvshowEmployeesModel != null)
                {
                    cache.Set($"AllGuestsModel", allTvshowEmployeesModel, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
            }
            return allTvshowEmployeesModel;
        }
        public async Task<AllTvshowGuestsModel> GetAllTvshowGuestsModel()
        {
            AllTvshowGuestsModel allTvshowGuestsModel = null;

            if (!cache.TryGetValue($"AllTvshowGuestsModel", out allTvshowGuestsModel))
            {
                allTvshowGuestsModel = new AllTvshowGuestsModel();
                allTvshowGuestsModel.tvshowGuest = await db.TvshowGuests.ToListAsync();
                if (allTvshowGuestsModel != null)
                {
                    cache.Set($"AllTvshowGuestsModel", allTvshowGuestsModel, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
            }
            return allTvshowGuestsModel;
        }
        public async Task SaveShow(TvshowModel model, int showId)
        {
            Tvshow tvshow = null;
            if(showId > 0)
            {
                tvshow = await db.Tvshows.FirstOrDefaultAsync(t => t.ShowId == showId);
            }
            if (tvshow == null)
            {
                CreateShow(model);
            }
            else
            {
                UpdateShow(model, tvshow);
            }
            cache.Remove($"TvshowModel");
            List<TvshowModel> tvshowModels;
        }
        public async Task CreateShow(TvshowModel model)
        {
            Tvshow tvshow = new Tvshow()
            {
                GenreId = model.GenreId,
                Title = model.tvshow.Title,
                Description = model.tvshow.Description,
                Duration = model.tvshow.Duration,
                Rating = model.tvshow.Rating,
            };
            db.Tvshows.Add(tvshow);
            await db.SaveChangesAsync();
            int showId = tvshow.ShowId;
            if (showId != 0)
            {
                foreach (var employee in model.employee)
                {
                    TvshowEmployee tvshowEmployee = new TvshowEmployee()
                    {
                        ShowId = showId,
                        EmployeeId = employee.EmployeeId,
                    };
                    db.TvshowEmployees.Add(tvshowEmployee);
                }
                foreach (var guest in model.guest)
                {
                    TvshowGuest tvshowGuest = new TvshowGuest()
                    {
                        GuestId = guest.GuestId,
                        ShowId = showId,
                    };
                    db.TvshowGuests.Add(tvshowGuest);
                }
                await db.SaveChangesAsync();
            }
        }
        public async Task UpdateShow(TvshowModel model, Tvshow tvshow)
        {
            tvshow.Title = model.tvshow.Title;
            tvshow.Description = model.tvshow.Description;
            tvshow.Rating = model.tvshow.Rating;
            tvshow.Duration = model.tvshow.Duration;
            tvshow.GenreId = model.tvshow.GenreId;
            db.Tvshows.Update(tvshow);

            var employeeTvshow = db.TvshowEmployees.Where(e => e.ShowId == tvshow.ShowId).ToList();
            var employeeIdsInModel = model.employee.Select(e => e.EmployeeId).ToList();
            var employeesNotInModel = employeeTvshow.Where(e => !employeeIdsInModel.Contains(e.EmployeeId)).ToList();
            foreach(TvshowEmployee employee in employeesNotInModel)
            {
                db.TvshowEmployees.Remove(employee);
            }
            var employeesInModel = employeeTvshow.Where(e => employeeIdsInModel.Contains(e.EmployeeId)).ToList();
            var employeeIds = employeesInModel.Select(e => e.EmployeeId).ToList();
            var employeeIns = model.employee.Where(e => !employeeIds.Contains(e.EmployeeId)).ToList();
            foreach (var employee in employeeIns)
            {
                TvshowEmployee tvshowEmployee = new TvshowEmployee()
                {
                    ShowId = tvshow.ShowId,
                    EmployeeId = employee.EmployeeId,
                };
                db.TvshowEmployees.Add(tvshowEmployee);
            }

            var guestTvshow = db.TvshowGuests.Where(e => e.ShowId == tvshow.ShowId).ToList();
            var guestIdsInModel = model.guest.Select(g => g.GuestId).ToList();
            var guestNotInModel = guestTvshow.Where(g => !guestIdsInModel.Contains(g.GuestId));
            foreach(TvshowGuest guest in guestNotInModel)
            {
                db.TvshowGuests.Remove(guest);
            }
            var guestInModel = guestTvshow.Where(g => guestIdsInModel.Contains(g.GuestId));
            var guestIds = guestInModel.Select(g => g.GuestId).ToList();
            var guestIns = model.guest.Where(g => !employeeIds.Contains(g.GuestId)).ToList();
            foreach(Guest guest in guestIns)
            {
                TvshowGuest tvshowGuest = new TvshowGuest()
                {
                    GuestId = guest.GuestId,
                    ShowId = tvshow.ShowId,
                };
                db.TvshowGuests.Add(tvshowGuest);
            }

            await db.SaveChangesAsync();
        }
        public async Task DeleteShow(int showId)
        {
            var show = await db.Tvshows.FirstOrDefaultAsync(t => t.ShowId == showId);
            if(show != null)
            {
                var tvshowEmployee = db.TvshowEmployees.Where(t => t.ShowId == showId);
                var tvshowGuest = db.TvshowGuests.Where(t => t.ShowId == showId);
                var scheduleTvshow = db.ScheduleTvshows.Where(t => t.ShowId == showId);// Удаляем все записи
                cache.Remove($"TvshowModel");
                db.TvshowEmployees.RemoveRange(tvshowEmployee);
                db.TvshowGuests.RemoveRange(tvshowGuest);
                db.ScheduleTvshows.RemoveRange(scheduleTvshow);
                db.Tvshows.Remove(show);

                // Сохраняем изменения в базе данных
                await db.SaveChangesAsync();
            }
        }
    }
}
