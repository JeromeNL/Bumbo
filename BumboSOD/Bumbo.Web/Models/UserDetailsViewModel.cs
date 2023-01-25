using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Bumbo.Data.Models;
using Bumbo.Data.ValidationAttributes;
using Microsoft.AspNetCore.Identity;

namespace Bumbo.Web.Models;

public class UserDetailsViewModel
{
    public string Id { get; set; }
    public string? FullName { get; set; }

    [Required(ErrorMessage = "Verplicht veld")]
    [MaxLength(30, ErrorMessage = "Maximaal 30 characters.")]
    [DisplayName("Voornaam")]
    public string FirstName { get; set; }

    [MaxLength(30, ErrorMessage = "Maximaal 30 characters.")]
    [DisplayName("Tussenvoegsel")]
    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Verplicht veld")]
    [MaxLength(30, ErrorMessage = "Maximaal 30 characters.")]
    [DisplayName("Achternaam")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Verplicht veld")]
    [DisplayName("Geboortedatum")]
    [BirthDateMayNotBeInTheFuture]
    public DateTime BirthDate { get; set; }

    [DisplayName("In dienst sinds")]
    public DateTime RegistrationDate { get; set; }

    [DisplayName("Adres")]
    public Address? Address { get; set; }

    [Required(ErrorMessage = "Verplicht veld")]
    [MaxLength(10)]
    [DisplayName("Postcode")]
    public string Zipcode { get; set; }

    [Required(ErrorMessage = "Verplicht veld")]
    [MaxLength(100)]
    [DisplayName("Straat")]
    [RegularExpression(@"^[^\W\d_]+\.?(?:[-\s][^\W\d_]+\.?)*$", ErrorMessage = "Geef een geldige straatnaam op")]
    public string Street { get; set; }

    [Required(ErrorMessage = "Verplicht veld")]
    [DisplayName("Huisnummer")]
    [Range(1, 1000, ErrorMessage = "Huisnummer moet onder de duizend zijn")]
    public int HouseNumber { get; set; }

    [Required(ErrorMessage = "Verplicht veld")]
    [MaxLength(150, ErrorMessage = "Maximaal 150 charactesr")]
    [DisplayName("Woonplaats")]
    public string City { get; set; }

    [Required(ErrorMessage = "Verplicht veld")]
    [DisplayName("Loonschaal")]
    [Range(1, 10, ErrorMessage = "Waarde moet tussen de 1 en 10 zijn")]
    public int PayoutScale { get; set; }

    [Required(ErrorMessage = "Verplicht veld")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Verplicht veld")]
    [Phone]
    [DisplayName("Telefoonnummer")]
    public string PhoneNumber { get; set; }

    [DisplayName("Filiaal")]
    public Branch? Branch { get; set; }

    public List<Department>? CurrentDepartments { get; set; }
    public List<string>? CurrentRoles { get; set; }
    public List<IdentityRole>? AllRoles { get; set; }
    public List<Department>? AllDepartments { get; set; }

    public string? SelectedRole { get; set; }
    public List<int> SelectedDepartments { get; set; }
    public int BranchId { get; set; }
}