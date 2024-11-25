using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourceWork.Data;
using CourceWork.Models;
using CourceWork.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace CourceWork.Tests
{
    public class ScheduleServiceTests
    {
        private ScheduleService _service;
        private Mock<IMemoryCache> _cacheMock;
        private DatabaseContext _dbContext;

        public ScheduleServiceTests()
        {
            // Настраиваем InMemory базу данных
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new DatabaseContext(options);

            // Настраиваем мок для IMemoryCache
            _cacheMock = new Mock<IMemoryCache>();

            // Создаем экземпляр сервиса
            _service = new ScheduleService(_dbContext, _cacheMock.Object);
        }
        [Fact]
        public async Task ChooseWeek_Returns_Valid_ScheduleModel()
        {
            // Arrange
            var testDate = new DateOnly(2024, 11, 25);
            var schedule = new Schedule
            {
                ScheduleId = 1,
                Date = testDate
            };

            // Добавляем данные в тестовую базу
            _dbContext.Schedules.Add(schedule);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.ChooseWeek(testDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testDate, result.findDate);
            Assert.NotNull(result.startDate);
            Assert.NotNull(result.endDate);
        }
        [Fact]
        public async Task ChooseDay_Returns_Valid_DayModel()
        {
            var testDate = new DateOnly(2024, 11, 25);
            var schedule = new Schedule
            {
                ScheduleId = 1,
                Date = testDate
            };

            var tvShow = new Tvshow
            {
                ShowId = 1,
                Title = "Test Show",
                Duration = 60
            };

            var scheduleTvShow = new ScheduleTvshow
            {
                ScheduleTvshowId = 1,
                ScheduleId = 1,
                ShowId = 1,
                SequenceNumber = 1
            };

            // Добавляем данные в тестовую базу
            _dbContext.Schedules.Add(schedule);
            _dbContext.Tvshows.Add(tvShow);
            _dbContext.ScheduleTvshows.Add(scheduleTvShow);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.ChooseDay(testDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(schedule, result.schedules);
            Assert.Single(result.scheduleTvshows);
            Assert.NotEmpty(result.showTime);
        }
        [Fact]
        public async Task AddShowToSchedule_Adds_Show_To_Schedule()
        {
            var testDate = new DateTime(2024, 11, 25);
            var schedule = new Schedule
            {
                ScheduleId = 1,
                Date = new DateOnly(testDate.Year, testDate.Month, testDate.Day)
            };

            var tvShow = new Tvshow
            {
                ShowId = 1,
                Title = "Test Show",
                Duration = 60
            };

            // Добавляем данные в тестовую базу
            _dbContext.Schedules.Add(schedule);
            _dbContext.Tvshows.Add(tvShow);
            await _dbContext.SaveChangesAsync();

            // Act
            await _service.AddShowToSchedule(schedule.ScheduleId, tvShow.ShowId, 1, testDate);

            // Assert
            var scheduleTvShow = await _dbContext.ScheduleTvshows.FirstOrDefaultAsync();
            Assert.NotNull(scheduleTvShow);
            Assert.Equal(schedule.ScheduleId, scheduleTvShow.ScheduleId);
            Assert.Equal(tvShow.ShowId, scheduleTvShow.ShowId);
            Assert.Equal(1, scheduleTvShow.SequenceNumber);
        }
        [Fact]
        public async Task DeleteShowToSchedule_Removes_ScheduleTvShow()
        {
            // Arrange
            var testDate = new DateOnly(2024, 11, 25);
            var schedule = new Schedule
            {
                ScheduleId = 1,
                Date = testDate
            };

            var scheduleTvShow = new ScheduleTvshow
            {
                ScheduleTvshowId = 1,
                ScheduleId = 1,
                ShowId = 1,
                SequenceNumber = 1
            };

            // Добавляем данные в тестовую базу
            _dbContext.Schedules.Add(schedule);
            _dbContext.ScheduleTvshows.Add(scheduleTvShow);
            await _dbContext.SaveChangesAsync();

            // Act
            var resultDate = await _service.DeleteShowToSchedule(scheduleTvShow.ScheduleTvshowId);

            // Assert
            var deletedScheduleTvShow = await _dbContext.ScheduleTvshows
                .FirstOrDefaultAsync(s => s.ScheduleTvshowId == scheduleTvShow.ScheduleTvshowId);

            Assert.Null(deletedScheduleTvShow);
            Assert.Equal(testDate, resultDate);
        }
    }
}