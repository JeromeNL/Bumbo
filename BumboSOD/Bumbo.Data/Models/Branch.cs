using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bumbo.Data.Models;

public class Branch
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string? Name { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    [Required]
    public decimal ShelfLength { get; set; }

    [Required]
    public Address? Address { get; set; }

    public int? AddressId { get; set; }

    public List<OpeningHours>? OpeningHours { get; set; } = new();

    public List<WorkStandards>? WorkStandards { get; set; } = new();

    public List<ApplicationUser>? Employees { get; set; } = new();

    // per dag aantal klanten en coli
    public List<HistoricalData>? HistoricalData { get; set; } = new();

    // per dag per afdeling aantal manuren verwacht
    public List<Prognosis>? Prognoses { get; set; } = new();
}