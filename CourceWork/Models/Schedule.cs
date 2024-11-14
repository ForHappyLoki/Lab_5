using System;
using System.Collections.Generic;

namespace CourceWork.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int WeekNumber { get; set; }

    public int MonthNumber { get; set; }

    public int Year { get; set; }

    public DateTime? EndTime { get; set; }

    public virtual ICollection<ScheduleTvshow> ScheduleTvshows { get; set; } = new List<ScheduleTvshow>();
}
