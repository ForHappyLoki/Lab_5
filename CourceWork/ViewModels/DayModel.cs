using CourceWork.Models;

namespace CourceWork.ViewModels
{
    public class DayModel
    {
        public Schedule schedules { get; set; }
        public List<ShowTime> showTime { get; set; } = new List<ShowTime>();
        public List<ScheduleTvshow> scheduleTvshows { get; set; } = new List<ScheduleTvshow>();
        public AllShowsModel tvshows { get; set; } = new AllShowsModel();
    }
    public class AllShowsModel
    {
        public List<Tvshow> tvshows { get; set; } = new List<Tvshow>();
    }
}
