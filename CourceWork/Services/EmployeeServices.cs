using CourceWork.Data;
using CourceWork.Models;
using CourceWork.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CourceWork.Services
{
    public class EmployeeServices(DatabaseContext databaseContext, IMemoryCache memory)
    {
        private DatabaseContext db = databaseContext;
        public IMemoryCache cache = memory;
        public async Task<EmployeeModel> GetEmployeeModel(int employeeId)
        {
            EmployeeModel employeeModel = null;
            if (!cache.TryGetValue($"employeeId_{employeeId}", out employeeModel))
            {
                employeeModel = new EmployeeModel();
                employeeModel.employee = db.Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
                if(employeeModel != null)
                {
                    List<TvshowEmployee> tvshowEmployees = new List<TvshowEmployee>();
                    tvshowEmployees = await db.TvshowEmployees.Where(e => e.EmployeeId == employeeId).ToListAsync();
                    employeeModel.tvshowEmployees = tvshowEmployees;

                    var tvshowIds = tvshowEmployees.Select(e => e.ShowId).ToList();

                    List<Tvshow> tvshows = await db.Tvshows
                        .Where(t => tvshowIds.Contains(t.ShowId))
                        .ToListAsync();
                    tvshows.OrderBy(e => e.Title).ToList();
                    employeeModel.tvshows = tvshows;

                    List<ScheduleTvshow> scheduleTvshows = await db.ScheduleTvshows
                        .Where(t => tvshowIds.Contains(t.ShowId))
                        .ToListAsync();
                    employeeModel.scheduleTvshows = scheduleTvshows;

                    var scheduleIds = scheduleTvshows.Select(s => s.ScheduleId).ToList();

                    List<Schedule> schedules = await db.Schedules
                        .Where(t => scheduleIds.Contains(t.ScheduleId))
                        .ToListAsync();
                    schedules.OrderBy(s => s.Date).ToList();
                    employeeModel.schedules = schedules;
                    if (employeeModel != null)
                    {
                        cache.Set($"user_{employeeModel.employee.EmployeeId}", employeeModel,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                    }
                }
            }
            return employeeModel;
        }
    }
}
