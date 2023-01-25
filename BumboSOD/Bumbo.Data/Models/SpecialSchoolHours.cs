using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bumbo.Data.Models;

public class SpecialSchoolHours
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public ApplicationUser Employee { get; set; }

    public string EmployeeId { get; set; }

    [Required]
    public DateTime Start { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    [Required]
    public decimal Hours { get; set; }
}