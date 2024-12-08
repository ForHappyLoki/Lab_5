using CourceWork.Models;
using Microsoft.IdentityModel.Tokens;

namespace CourceWork.ViewModels
{
    public class EmployeeModel
    {
        public Employee employee { get; set; }
        public List<TvshowEmployee> tvshowEmployees { get; set; }
        public List<ScheduleTvshow> scheduleTvshows { get; set; }
        public List<Tvshow> tvshows { get; set; }
        public List<Schedule> schedules { get; set; }
        public List<int> GetUniqueDate()
        {
            var uniqueYears = schedules
                .Select(s => s.Date.Value.Year) // Извлекаем год из каждой даты
                .Distinct() // Получаем уникальные года
                .OrderBy(year => year) // Сортируем по возрастанию
                .ToList(); // Преобразуем в список
            return uniqueYears;
        }
        public List<String> GetEmployeeShow(int year, int month)
        {
            // Фильтруем расписания по году и месяцу
            var filteredSchedules = schedules
                .Where(s => s.Date.Value.Year == year && s.Date.Value.Month == month)
                .ToList();

            // Получаем список строк с названиями передач и их датами
            var result = (from schedule in filteredSchedules
                          join scheduleTvshow in scheduleTvshows on schedule.ScheduleId equals scheduleTvshow.ScheduleId
                          join tvshow in tvshows on scheduleTvshow.ShowId equals tvshow.ShowId
                          select $"{tvshow.Title,-30} - {tvshow.Duration,-5} минут - {schedule.Date.Value.ToString(),-10}")
                         .ToList();
            return result;
        }
        public int GetWorkTime(int year, int month)
        {
            // Фильтруем расписания по году и месяцу
            var filteredSchedules = schedules
                .Where(s => s.Date.Value.Year == year && s.Date.Value.Month == month)
                .ToList();

            // Получаем список строк с названиями передач и их датами
            var totalDuration = (from schedule in filteredSchedules
                                 join scheduleTvshow in scheduleTvshows on schedule.ScheduleId equals scheduleTvshow.ScheduleId
                                 join tvshow in tvshows on scheduleTvshow.ShowId equals tvshow.ShowId
                                 select tvshow.Duration)
                                 .Sum();
            return totalDuration;
        }
    }
}
