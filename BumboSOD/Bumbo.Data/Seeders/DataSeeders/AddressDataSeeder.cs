using Bogus;
using Bumbo.Data.Models;
using Bumbo.Data.Seeders.Interfaces;

namespace Bumbo.Data.Seeders.DataSeeders;

public abstract class AddressDataSeeder : IDataSeeder
{
    private static int Id { get; set; }

    public static List<Address> GenerateAddresses(int amount, string locale)
    {
        var addressFaker = new Faker<Address>(locale)
            .RuleFor(o => o.Id, _ => ++Id)
            .RuleFor(o => o.Zipcode, f => f.Address.ZipCode())
            .RuleFor(o => o.Street, f => f.Address.StreetName())
            .RuleFor(o => o.HouseNumber, f => f.Random.Int(9, 233))
            .RuleFor(o => o.City, f => f.Address.City());
        return addressFaker.Generate(amount);
    }
}