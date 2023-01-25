using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bumbo.Data.Models;

public class Shift
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public ApplicationUser Employee { get; set; }

    public string EmployeeId { get; set; }

    [Required]
    public Department Department { get; set; }

    public int? DepartmentId { get; set; }

    [Required]
    public DateTime Start { get; set; }

    [Required]
    public DateTime End { get; set; }

    [Required]
    public bool IsPublished { get; set; }

    [Required]
    public bool IsIll { get; set; }

    public ExchangeRequest? ExchangeRequest { get; set; }
}