using Bumbo.Data.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bumbo.Data.Models;

public class WorkStandards
{
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [DisplayName("Taak")]
    public WorkStandardTypes Task { get; set; }

    [Required]
    [DisplayName("Vereiste tijd in minuten")]
    [Column(TypeName = "decimal(5,1)")]
    public decimal RequiredTimeInMinutes { get; set; }

    [Required]
    [DisplayName("Datum invoering")]
    public DateTime DateEntered { get; set; }

    [Required]
    [DisplayName("Filiaal")]
    public Branch? Branch { get; set; }

    public int? BranchId { get; set; }
}