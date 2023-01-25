using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bumbo.Data.Models;

public class Prognosis
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int DepartmentId { get; set; }
    public Department Department { get; set; }

    public int BranchId { get; set; }
    public Branch Branch { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal ManHoursExpected { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    public bool CalculationSuccessful { get; set; }

    public DateTime CreationDate { get; set; } = DateTime.Now;
}