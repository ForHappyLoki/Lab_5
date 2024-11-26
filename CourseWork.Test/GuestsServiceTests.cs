using CourceWork.Data;
using CourceWork.Models;
using CourceWork.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class GuestsServiceTests
{
    private readonly GuestsService _guestsService;
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly DbContextOptions<DatabaseContext> _dbContextOptions;

    public GuestsServiceTests()
    {
        // Настройка in-memory базы данных
        _dbContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        // Мок кеша
        _mockCache = new Mock<IMemoryCache>();

        // Инициализация сервиса
        using (var context = new DatabaseContext(_dbContextOptions))
        {
            _guestsService = new GuestsService(context, _mockCache.Object);
        }
    }

    [Fact]
    public async Task GetGuests_ShouldReturnGuests_WhenCacheIsEmpty()
    {
        // Arrange
        var guestList = new List<Guest>
        {
            new Guest { GuestId = 1, FullName = "John Doe" },
            new Guest { GuestId = 2, FullName = "Jane Doe" }
        };

        using (var context = new DatabaseContext(_dbContextOptions))
        {
            context.Guests.AddRange(guestList);
            context.SaveChanges();
        }

        // Act
        var guests = await _guestsService.GetGuests();

        // Assert
        Assert.Equal(2, guests.Count);
        Assert.Equal("John Doe", guests[0].FullName);
    }

    [Fact]
    public async Task Editing_ShouldAddNewGuest_WhenGuestIdIsZero()
    {
        // Arrange
        var newGuest = new Guest { GuestId = 0, FullName = "New Guest" };

        // Act
        await _guestsService.Editing(newGuest);

        // Assert
        using (var context = new DatabaseContext(_dbContextOptions))
        {
            var guests = await context.Guests.ToListAsync();
            Assert.Single(guests);
            Assert.Equal("New Guest", guests[0].FullName);
        }
    }

    [Fact]
    public async Task Editing_ShouldUpdateExistingGuest_WhenGuestIdIsPositive()
    {
        // Arrange
        var existingGuest = new Guest { GuestId = 1, FullName = "Existing Guest" };

        using (var context = new DatabaseContext(_dbContextOptions))
        {
            context.Guests.Add(existingGuest);
            context.SaveChanges();
        }

        var updatedGuest = new Guest { GuestId = 1, FullName = "Updated Guest" };

        // Act
        await _guestsService.Editing(updatedGuest);

        // Assert
        using (var context = new DatabaseContext(_dbContextOptions))
        {
            var guest = await context.Guests.FirstOrDefaultAsync(g => g.GuestId == 1);
            Assert.Equal("Updated Guest", guest.FullName);
        }
    }

    [Fact]
    public async Task Delete_ShouldRemoveGuest_WhenGuestExists()
    {
        // Arrange
        var guestToDelete = new Guest { GuestId = 1, FullName = "Guest to Delete" };
        var guestTvshowToDelete = new TvshowGuest { GuestId = 1, ShowId = 1 };

        using (var context = new DatabaseContext(_dbContextOptions))
        {
            context.Guests.Add(guestToDelete);
            context.TvshowGuests.Add(guestTvshowToDelete);
            context.SaveChanges();
        }

        // Act
        await _guestsService.Delete(1);

        // Assert
        using (var context = new DatabaseContext(_dbContextOptions))
        {
            var guest = await context.Guests.FindAsync(0);
            Assert.Null(guest);
        }
    }
}