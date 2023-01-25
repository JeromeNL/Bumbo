using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bumbo.Data.Models;

public class Address
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string Zipcode { get; set; }

    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[^\W\d_]+\.?(?:[-\s][^\W\d_]+\.?)*$", ErrorMessage = "Geef een geldige straatnaam op")]
    public string Street { get; set; }

    [Required]
    [Range(1, 1000, ErrorMessage = "Huisnummer moet onder de duizend zijn")]
    public int HouseNumber { get; set; }

    [Required]
    [MaxLength(150)]
    public string City { get; set; }
}