using CourceWork.Models;

namespace CourceWork.ViewModels
{
    public class ScheduleModel
    {
        public Schedule[] schedules { get; set; } = new Schedule[7];
        public List<ScheduleTvshow>[] scheduleTvshows { get; set; } = new List<ScheduleTvshow>[7];
        public DateOnly startDate { get; set; }
        public DateOnly findDate { get; set; }
        public DateOnly endDate { get; set; }
        public TimeOnly endTime { get; set; } = new TimeOnly(23, 59);
        public List<ShowTime>[] showTime { get; set; } = new List<ShowTime>[7];
    }
    public class ShowTime
    {
        public TimeOnly scheduleTvshowsTime { get; set; }
        public string showName { get; set; }
        public int showId { get; set; }
    }
}
