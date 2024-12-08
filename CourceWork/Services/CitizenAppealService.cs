using CourceWork.Data;
using CourceWork.Models;
using CourceWork.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Xunit.Abstractions;

namespace CourceWork.Services
{
    public class CitizenAppealService(DatabaseContext databaseContext, IMemoryCache memory)
    {
        private DatabaseContext db = databaseContext;
        public IMemoryCache cache = memory;
        public async Task<AllShowsModel> GetShows()
        {
            AllShowsModel model; 
            if (!cache.TryGetValue($"AllShowsModel", out model))
            {
                model = new AllShowsModel();
                model.tvshows = await db.Tvshows.ToListAsync();

                if (model.tvshows != null)
                {
                    cache.Set($"AllShowsModel", model,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }
            return model;
        }
        public async Task SaveCitizenAppeal(CitizenAppealModel model)
        {
            if (model != null)
            {
                CitizenAppeal citizenAppeal = model.CitizenAppeal;
                citizenAppeal.ShowId = model.ShowId;
                citizenAppeal.DateAppeal = DateTime.Now;
                db.CitizenAppeals.Add(citizenAppeal);
                await db.SaveChangesAsync();
                cache.Remove("CitizenAppealModel");
            }
        }
        public async Task<List<CitizenAppealModel>> GetModel()
        {
            List<CitizenAppealModel> citizenAppealModels;

            if (!cache.TryGetValue($"CitizenAppealModel", out citizenAppealModels))
            {
                citizenAppealModels = new List<CitizenAppealModel>();
                List<CitizenAppeal> citizenAppeals = await db.CitizenAppeals
                    .OrderBy(appeal => appeal.DateAppeal) // Сортировка по возрастанию
                    .ToListAsync();
                foreach (var appelal in citizenAppeals)
                {
                    CitizenAppealModel citizenAppealModel = new CitizenAppealModel();
                    citizenAppealModel.CitizenAppeal = appelal;
                    citizenAppealModels.Add(citizenAppealModel);
                }
                if (citizenAppealModels != null)
                {
                    cache.Set($"CitizenAppealModel", citizenAppealModels,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }
            return citizenAppealModels;
        }
        [HttpDelete]
        public async Task DeleteAppeal(int appealId)
        {
            if(appealId > 0)
            {
                CitizenAppeal citizenAppeal = db.CitizenAppeals.FirstOrDefault(c => c.AppealId == appealId);
                if(citizenAppeal != null)
                {
                    db.CitizenAppeals.Remove(citizenAppeal);
                    await db.SaveChangesAsync();
                    cache.Remove("CitizenAppealModel");
                }
            }
        }
    }
}
