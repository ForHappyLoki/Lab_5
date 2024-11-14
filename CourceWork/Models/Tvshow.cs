using System;
using System.Collections.Generic;

namespace CourceWork.Models;

public partial class Tvshow
{
    public int ShowId { get; set; }

    public int GenreId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int Duration { get; set; }

    public decimal? Rating { get; set; }

    public virtual ICollection<CitizenAppeal> CitizenAppeals { get; set; } = new List<CitizenAppeal>();

    public virtual Genre Genre { get; set; } = null!;

    public virtual ICollection<ScheduleTvshow> ScheduleTvshows { get; set; } = new List<ScheduleTvshow>();

    public virtual ICollection<TvshowEmployee> TvshowEmployees { get; set; } = new List<TvshowEmployee>();

    public virtual ICollection<TvshowGuest> TvshowGuests { get; set; } = new List<TvshowGuest>();
}
