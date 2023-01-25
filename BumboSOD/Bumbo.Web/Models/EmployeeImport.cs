using CsvHelper.Configuration.Attributes;

namespace Bumbo.Web.Models;

public class EmployeeImport
{
    [Name("BID")]
    public string Bid { get; set; }

    [Name("Vn")]
    public string Voornaam { get; set; }

    [Name("Tv")]
    public string Tussenvoegsel { get; set; }

    [Name("An")]
    public string Achternaam { get; set; }

    [Name("Geboortedatum")]
    public DateTime Geboortedatum { get; set; }

    [Name("Postcode")]
    public string Postcode { get; set; }

    [Name("Huisnummer")]
    public int Huisnummer { get; set; }

    [Name("Telefoon")]
    public string Telefoon { get; set; }

    [Name("Email")]
    public string Email { get; set; }

    [Name("In dienst")]
    public DateTime DatumInDienst { get; set; }

    [Name("Functie")]
    public string? Functie { get; set; }

    [Name("KAS")]
    public string? Kassa { get; set; }

    [Name("VERS")]
    public string? Vers { get; set; }

    [Name("VAK")]
    public string? Vakkenvullen { get; set; }
}