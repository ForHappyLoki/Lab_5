using System;
using System.Collections.Generic;

namespace CourceWork.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly HireDate { get; set; }

    public string? Position { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public virtual ICollection<TvshowEmployee> TvshowEmployees { get; set; } = new List<TvshowEmployee>();
}
