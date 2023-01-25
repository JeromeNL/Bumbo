using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bumbo.Data.Models;

public class HistoricalData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public Branch? Branch { get; set; }

    public int? BranchId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public int AmountCustomers { get; set; }

    public int AmountColi { get; set; }
}