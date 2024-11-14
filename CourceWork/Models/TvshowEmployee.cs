using System;
using System.Collections.Generic;

namespace CourceWork.Models;

public partial class TvshowEmployee
{
    public int ShowId { get; set; }

    public int EmployeeId { get; set; }

    public int TvshowEmployeeId { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Tvshow Show { get; set; } = null!;
}
