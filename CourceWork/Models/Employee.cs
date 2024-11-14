using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

    [NotMapped] // Это свойство не сохраняется в базе данных
    public string HireDateString
    {
        get => HireDate.ToString("yyyy-MM-dd"); // Формат для input типа date
        set => HireDate = DateOnly.Parse(value); // Преобразование строки в DateOnly
    }
    public Employee()
    {
        HireDate = DateOnly.FromDateTime(DateTime.Now);
    }
}
