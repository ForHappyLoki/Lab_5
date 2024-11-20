using CourceWork.Models;

namespace CourceWork.ViewModels
{
    public class ScheduleModel
    {
        public Schedule[] schedules { get; set; } = new Schedule[7];
        public ScheduleTvshow[] scheduleTvshows { get; set; }
    }
}
