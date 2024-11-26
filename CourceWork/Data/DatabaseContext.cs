using System;
using System.Collections.Generic;
using CourceWork.Models;
using Microsoft.EntityFrameworkCore;

namespace CourceWork.Data;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CitizenAppeal> CitizenAppeals { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Guest> Guests { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<ScheduleTvshow> ScheduleTvshows { get; set; }

    public virtual DbSet<Tvshow> Tvshows { get; set; }

    public virtual DbSet<TvshowEmployee> TvshowEmployees { get; set; }

    public virtual DbSet<TvshowGuest> TvshowGuests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        DatabaseContextHelpers.
                CitizenAppealConfiguration(modelBuilder);
        DatabaseContextHelpers.
                EmployeeConfiguration(modelBuilder);
        DatabaseContextHelpers.
                GenreConfiguration(modelBuilder);
        DatabaseContextHelpers.
                GuestConfiguration(modelBuilder);
        DatabaseContextHelpers.
                ScheduleConfiguration(modelBuilder);
        DatabaseContextHelpers.
                ScheduleTvshowConfiguration(modelBuilder);
        DatabaseContextHelpers.
                TvshowConfiguration(modelBuilder);
        DatabaseContextHelpers.
                TvshowEmployeeConfiguration(modelBuilder);
        DatabaseContextHelpers.
                TvshowGuestConfiguration(modelBuilder);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
