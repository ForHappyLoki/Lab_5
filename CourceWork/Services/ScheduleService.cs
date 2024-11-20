using CourceWork.Data;
using Microsoft.Extensions.Caching.Memory;

namespace CourceWork.Services
{
    public class ScheduleService
    {
        private DatabaseContext db;
        public IMemoryCache cache;
        public ScheduleService(DatabaseContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }
    }
}
