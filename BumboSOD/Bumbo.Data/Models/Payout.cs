using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bumbo.Data.Models;

public class Payout
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public ApplicationUser? Employee { get; set; }

    public string? EmployeeId { get; set; }

    [Required]
    [Range(1, 53)]
    public int WeekNumber { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    [Required]
    public decimal PayedHours { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal Bonus { get; set; }
}