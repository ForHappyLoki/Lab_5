using CourceWork.Data;
using CourceWork.Models;
using CourceWork.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CourceWork.Services
{
    public class GuestsService
    {
        private DatabaseContext db;
        public IMemoryCache cache;
        public GuestsService(DatabaseContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }
        public async Task<List<Guest>> GetGuests()
        {
            List<Guest> guests;
            if (!cache.TryGetValue($"Guests", out guests))
            {
                guests = await db.Guests.ToListAsync();
                if (guests != null)
                {
                    cache.Set($"Guests", guests,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }
            return guests;
        }
        public async Task Editing(Guest guest)
        {
            if (guest != null)
            {
                if(guest.GuestId > 0)
                {
                    await EditGuest(guest);
                }
                else
                {
                    await SaveGuest(guest);
                }
                cache.Remove($"Guests");  
                await db.SaveChangesAsync();
            }
        }
        private async Task SaveGuest(Guest guest)
        {
            if(!db.Guests.Any(g =>g.GuestId == guest.GuestId))
            {
                db.Guests.Add(guest);
            }
        }
        private async Task EditGuest(Guest guest)
        {
            if (guest != null)
            {
                var trueGuest = await db.Guests.FirstOrDefaultAsync(g => g.GuestId == guest.GuestId);
                trueGuest.FullName = guest.FullName;
            }
        }
    }
}
