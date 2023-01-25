using Bumbo.Data.Models.Enums;

namespace Bumbo.Web.Models;

public class ExportClockedHoursCsvViewModel
{
    public string EmployeeUuid { get; set; }
    public string EmployeeName { get; set; }
    public double TotalHours { get; set; }
    public Allowances Allowance { get; set; }
}