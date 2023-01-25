using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Bumbo.Data.Models;

public class ApplicationUser : IdentityUser
{
    [Required(ErrorMessage = "Verplicht veld")]
    [MaxLength(30, ErrorMessage = "Maximaal 30 characters.")]
    [PersonalData]
    [DisplayName("Voornaam")]
    public string FirstName { get; set; }

    [MaxLength(30, ErrorMessage = "Maximaal 30 characters.")]
    [PersonalData]
    [DisplayName("Tussenvoegsel")]
    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Verplicht veld")]
    [MaxLength(30, ErrorMessage = "Maximaal 30 characters.")]
    [PersonalData]
    [DisplayName("Achternaam")]
    public string LastName { get; set; }

    [NotMapped]
    public string FullName => FirstName + " " + (MiddleName != null ? MiddleName + " " : "") + LastName;

    [Required(ErrorMessage = "Verplicht veld")]
    [DisplayName("Geboortedatum")]
    public DateTime BirthDate { get; set; }

    // Default value set in BumboDbContext
    [Required(ErrorMessage = "Verplicht veld")]
    [DisplayName("Datum indiensttreding")]
    public DateTime RegistrationDate { get; set; }

    [Required(ErrorMessage = "Verplicht veld")]
    [PersonalData]
    [DisplayName("Adres")]
    public Address? Address { get; set; }

    public int AddressId { get; set; }

    [Required(ErrorMessage = "Verplicht veld")]
    [Range(1, 10, ErrorMessage = "Waarde moet tussen de 1 en 10 zijn")]
    [DisplayName("Loonschaal")]
    public int PayoutScale { get; set; }

    public List<ApplicationUserDepartment>? Departments { get; set; } = new();

    public Branch? Branch { get; set; }

    public int? BranchId { get; set; }

    public List<Shift>? Shifts { get; set; }

    public List<ClockedHours>? ClockedHours { get; set; }

    public List<StandardAvailability>? StandardAvailabilities { get; set; }

    public List<SpecialAvailability>? SpecialAvailabilities { get; set; }
}