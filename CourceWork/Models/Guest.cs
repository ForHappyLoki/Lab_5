using System;
using System.Collections.Generic;

namespace CourceWork.Models;

public partial class Guest
{
    public int GuestId { get; set; }

    public string FullName { get; set; } = null!;

    public virtual ICollection<TvshowGuest> TvshowGuests { get; set; } = new List<TvshowGuest>();
}
