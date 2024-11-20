using System;
using System.Collections.Generic;

namespace CourceWork.Models;

public partial class ScheduleTvshow
{
    public int ScheduleTvshowId { get; set; }

    public int ScheduleId { get; set; }

    public int ShowId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int? SequenceNumber { get; set; }

    public virtual Schedule Schedule { get; set; } = null!;

    public virtual Tvshow Show { get; set; } = null!;
}
