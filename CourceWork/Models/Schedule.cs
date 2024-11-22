using System;
using System.Collections.Generic;

namespace CourceWork.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public DateOnly? Date { get; set; }

    public virtual ICollection<ScheduleTvshow> ScheduleTvshows { get; set; } = new List<ScheduleTvshow>();
}
