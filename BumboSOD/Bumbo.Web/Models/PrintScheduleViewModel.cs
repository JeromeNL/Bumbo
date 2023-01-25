using Bumbo.Data.Models;

namespace Bumbo.Web.Models;

public class PrintScheduleViewModel
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string StartDateString => StartDate.ToString("dd-MM-yyyy");

    public string EndDateString => EndDate.ToString("dd-MM-yyyy");

    public Dictionary<ApplicationUser, List<Shift>> EmployeeShifts { get; set; }
}