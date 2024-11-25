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
    }
}