using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bumbo.Data.Models;

public class ClockedHours
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public ApplicationUser? Employee { get; set; }

    public string? EmployeeId { get; set; }

    [Required]
    public DateTime ClockedIn { get; set; }

    public DateTime? ClockedOut { get; set; }

    public bool IsApproved { get; set; }

    public bool IsRemunerated { get; set; }
}