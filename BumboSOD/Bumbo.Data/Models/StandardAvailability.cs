using System.ComponentModel.DataAnnotations;

namespace Bumbo.Data.Models;

public class StandardAvailability
{
    [Required]
    public string EmployeeId { get; set; }

    [Required]
    public ApplicationUser Employee { get; set; }

    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }
}