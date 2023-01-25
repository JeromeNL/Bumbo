using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bumbo.Data.Models;

public class ExchangeRequest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public ApplicationUser? OriginalUser { get; set; }

    public string? OriginalUserId { get; set; }

    public ApplicationUser? NewUser { get; set; }

    public string? NewUserId { get; set; }

    public bool? IsApprovedByManager { get; set; }

    public Shift? Shift { get; set; }

    public int? ShiftId { get; set; }
}