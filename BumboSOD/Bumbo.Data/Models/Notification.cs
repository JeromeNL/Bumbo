using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bumbo.Data.Models;

public class Notification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public ApplicationUser Employee { get; set; }

    public string EmployeeId { get; set; }

    public bool IsRead { get; set; }

    public string Title { get; set; }

    public string Message { get; set; }

    public string Href { get; set; }

    // Set this property to when the cause of the notification happened
    public DateTime HappenedAt { get; set; }
}