using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bumbo.Data.Models;

public class StandardSchoolHours
{
    [Required]
    public string EmployeeId { get; set; }

    [Required]
    public ApplicationUser? Employee { get; set; }

    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    [Range(0, 10)]
    [Required]
    public decimal Hours { get; set; }
}