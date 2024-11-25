using CourceWork.Data;
using CourceWork.Models;
using CourceWork.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CourceWork.Services
{
    public class ScheduleService
    {
        private CultureInfo cultureInfo;
        private Calendar calendar;
        private DatabaseContext db;
        public IMemoryCache cache;
        public ScheduleService(DatabaseContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
            cultureInfo = new CultureInfo("ru-RU");
            calendar = cultureInfo.Calendar;
        }
        public async Task<ScheduleModel> ChooseWeek(DateOnly date)
        {
            ScheduleModel scheduleModel = null;

            CalendarWeekRule rule = CalendarWeekRule.FirstDay; // Правило для первой недели
            DayOfWeek firstDayOfWeekRule = DayOfWeek.Monday; // Начало недели с понедельника

            DateTime startOfWeek = new DateTime(date.Year, date.Month, date.Day);
            int dayOfWeek = (int)startOfWeek.DayOfWeek;

            // Если день недели воскресенье (0), считаем его как 7
            if (dayOfWeek == 0)
            {
                dayOfWeek = 7;
            }

            // Теперь вычитаем (dayOfWeek - 1), чтобы получить понедельник
            DateTime firstDayOfWeek = startOfWeek.AddDays(-dayOfWeek + 1);

            int dayKey = calendar.GetWeekOfYear(startOfWeek, rule, firstDayOfWeekRule);
            if (!cache.TryGetValue($"{date.Year}_{dayKey}", out scheduleModel))
            {
                scheduleModel = new ScheduleModel();
                scheduleModel.startDate = new DateOnly(firstDayOfWeek.Year, firstDayOfWeek.Month, firstDayOfWeek.Day);
                DateTime lastDayOfWeek = firstDayOfWeek.AddDays(7);
                scheduleModel.endDate = new DateOnly(lastDayOfWeek.Year, lastDayOfWeek.Month, lastDayOfWeek.Day);
                // Заполняем расписание на 7 дней
                for (int i = 0; i < 7; i++)
                {
                    DateTime currentDate = firstDayOfWeek.AddDays(i);
                    DateOnly dateOnly = new DateOnly(currentDate.Year, currentDate.Month, currentDate.Day);
                    Schedule schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Date == dateOnly);
                    if (schedule != null)
                    {
                        scheduleModel.schedules[i] = schedule;
                        var scheduleTvshowsList = await db.ScheduleTvshows
                            .Where(tvshow => tvshow.ScheduleId == schedule.ScheduleId)
                            .ToListAsync();
                        scheduleTvshowsList = scheduleTvshowsList.OrderBy(tvshow => tvshow.SequenceNumber).ToList();
                        scheduleModel.scheduleTvshows[i] = scheduleTvshowsList;

                        TimeOnly nowTime = new TimeOnly(6, 0);
                        scheduleModel.showTime[i] = new List<ShowTime>();


                        foreach (var tvshow in scheduleTvshowsList)
                        {
                            Tvshow show = await db.Tvshows.FirstOrDefaultAsync(s => s.ShowId == tvshow.ShowId);
                            TimeSpan showDuration = TimeSpan.FromMinutes(show.Duration);
                            ShowTime showTime = new ShowTime()
                            {
                                scheduleTvshowsTime = nowTime,
                                showName = show.Title,
                                showId = show.ShowId
                            };
                            scheduleModel.showTime[i].Add(showTime);
                            nowTime = nowTime.Add(showDuration);
                        }
                    }
                }
                if (scheduleModel != null)
                {
                    cache.Set($"{date.Year}_{dayKey}", scheduleModel,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }
            scheduleModel.findDate = new DateOnly(date.Year, date.Month, date.Day);
            return scheduleModel;
        }
        public async Task<DayModel> ChooseDay(DateOnly date)
        {
            return await ChooseDay(new DateTime(date.Year, date.Month, date.Day));
        }
        public async Task<DayModel> ChooseDay(DateTime date)
        {
            Schedule schedule;
            if (!cache.TryGetValue($"schedule_{date.ToString()}", out schedule))
            {                
                DateOnly dateOnly = new DateOnly(date.Year, date.Month, date.Day);
                schedule = await db.Schedules.FirstOrDefaultAsync(s => s.Date == dateOnly);
                if (schedule == null)
                {
                    schedule = new Schedule()
                    {
                        Date = new DateOnly(date.Year, date.Month, date.Day)
                    };
                    db.Schedules.Add(schedule);
                    await db.SaveChangesAsync();
                }
                if (schedule != null)
                {
                    cache.Set($"schedule_{date.ToString()}", schedule,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }

            DayModel dayModel;
            if (!cache.TryGetValue($"dayModel_{date.Year}_{date.Month}_{date.Day}", out dayModel))
            {
                List<ScheduleTvshow> scheduleTvshowsList = await db.ScheduleTvshows
                    .Where(tvshow => tvshow.ScheduleId == schedule.ScheduleId)
                    .ToListAsync();
                scheduleTvshowsList = scheduleTvshowsList.OrderBy(tvshow => tvshow.SequenceNumber).ToList();

                List<ShowTime> showTime = new List<ShowTime>();
                TimeOnly nowTime = new TimeOnly(6, 0);
                foreach (var tvshow in scheduleTvshowsList)
                {
                    Tvshow show = await db.Tvshows.FirstOrDefaultAsync(s => s.ShowId == tvshow.ShowId);
                    TimeSpan showDuration = TimeSpan.FromMinutes(show.Duration);
                    ShowTime showTimeOne = new ShowTime()
                    {
                        scheduleTvshowsTime = nowTime,
                        showName = show.Title,
                        showId = show.ShowId
                    };
                    showTime.Add(showTimeOne);
                    nowTime = nowTime.Add(showDuration);
                }
                List<Tvshow> tvshowsList;
                if (!cache.TryGetValue($"tvshowsList", out tvshowsList))
                {
                    tvshowsList = await db.Tvshows.ToListAsync();

                    tvshowsList = tvshowsList.OrderBy(tvshow => tvshow.Title).ToList();
                    if (tvshowsList != null)
                    {
                        cache.Set($"tvshowsList", tvshowsList,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                    }
                }
                dayModel = new DayModel()
                {
                    schedules = schedule,
                    scheduleTvshows = scheduleTvshowsList,
                    showTime = showTime,
                    tvshows = new AllShowsModel()
                    {
                        tvshows = tvshowsList,
                    }
                };
                if (dayModel != null)
                {
                    cache.Set($"dayModel_{date.Year}_{date.Month}_{date.Day}", dayModel,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }
            return dayModel;
        }
        private async Task UpdateQueue(int order, DateTime date)
        {
            DayModel dayModel = await ChooseDay(date);
            bool free = true;
            int unFree = 0;
            bool stop = false;
            foreach(var record in dayModel.scheduleTvshows)
            {
                if (!free && record.SequenceNumber == unFree)
                {
                    record.SequenceNumber += 1;
                    unFree = record.SequenceNumber ?? 0;
                }
                else
                {
                    break;
                }
                if (record.SequenceNumber == order && free)
                {
                    free = false;
                    record.SequenceNumber += 1;
                    unFree = record.SequenceNumber ?? 0;
                }
            }
            if (!free)
            {
                await db.SaveChangesAsync();
            }
        }
        public async Task AddShowToSchedule(int scheduleId, int showId, int order, DateTime date)
        {
            await UpdateQueue(order, date);
            ScheduleTvshow scheduleTvshow = new ScheduleTvshow()
            {
                ScheduleId = scheduleId,
                ShowId = showId,
                SequenceNumber = order,
            };
            db.ScheduleTvshows.Add(scheduleTvshow);
            await db.SaveChangesAsync();
            CacheCleaner(date);
            cache.Remove($"schedule_{date.ToString()}");
            cache.Remove($"dayModel_{date.Year}_{date.Month}_{date.Day}");
        }
        public async Task EditShowToSchedule(List<ScheduleTvshow> scheduleTvshows, DateTime date)
        {
            for (int i = 0; i < scheduleTvshows.Count; i++)
            {
                // Retrieve the existing schedule
                ScheduleTvshow existingScheduleTvshow = await db.ScheduleTvshows
                    .FirstOrDefaultAsync(s => s.ScheduleTvshowId == scheduleTvshows[i].ScheduleTvshowId);

                if (existingScheduleTvshow != null)
                {
                    // Update the properties of the existing instance
                    existingScheduleTvshow.SequenceNumber = scheduleTvshows[i].SequenceNumber;

                    // No need to call Update, as existingScheduleTvshow is already tracked
                    await db.SaveChangesAsync();
                }
            }
            CacheCleaner(date);
            cache.Remove($"schedule_{date.ToString()}");
            cache.Remove($"dayModel_{date.Year}_{date.Month}_{date.Day}");
        }
        private void CacheCleaner(DateOnly date)
        {
            CalendarWeekRule rule = CalendarWeekRule.FirstDay; // Правило для первой недели
            DayOfWeek firstDayOfWeekRule = DayOfWeek.Monday; // Начало недели с понедельника
            DateTime startOfWeek = new DateTime(date.Year, date.Month, date.Day);
            int dayOfWeek = (int)startOfWeek.DayOfWeek; // 0 = Воскресенье, ..., 6 = Суббота
            DateTime firstDayOfWeek = startOfWeek.AddDays(-dayOfWeek + 1); // Понедельник

            int dayKey = calendar.GetWeekOfYear(startOfWeek, rule, firstDayOfWeekRule);
            cache.Remove($"{date.Year}_{dayKey}");
        }
        private void CacheCleaner(DateTime date)
        {
            CacheCleaner(new DateOnly(date.Year, date.Month, date.Day));
        }
        public async Task<DateOnly> DeleteShowToSchedule(int scheduleTvshowId)
        {
            var scheduleTvshowWithSchedule = await db.ScheduleTvshows
                .Include(s => s.Schedule) // Предполагаем, что есть навигационное свойство Schedule
                .FirstOrDefaultAsync(s => s.ScheduleTvshowId == scheduleTvshowId);

            var schedule = scheduleTvshowWithSchedule?.Schedule;
            DateOnly dateOnly = new DateOnly();
            if (schedule != null)
            {
                dateOnly = schedule.Date.GetValueOrDefault();
                if (scheduleTvshowWithSchedule != null )
                {
                    db.ScheduleTvshows.Remove(scheduleTvshowWithSchedule);
                    await db.SaveChangesAsync();
                    CacheCleaner(dateOnly);
                    cache.Remove($"schedule_{dateOnly.ToString()}");
                    cache.Remove($"dayModel_{dateOnly.Year}_{dateOnly.Month}_{dateOnly.Day}");
                }
            }
            return dateOnly;
        }
    }
}
