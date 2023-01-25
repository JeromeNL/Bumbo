using Bogus;
using Bumbo.Data.Models;
using Bumbo.Data.Seeders.Interfaces;

namespace Bumbo.Data.Seeders.DataSeeders;

public abstract class BranchDataSeeder : IDataSeeder
{
    private static int Id { get; set; }

    public static List<Branch> GenerateBranches(int amount, List<Address> addressList, string locale)
    {
        var branchFaker = new Faker<Branch>(locale)
            .RuleFor(o => o.Id, _ => ++Id)
            .RuleFor(o => o.Name, f => f.Company.CompanyName())
            .RuleFor(o => o.ShelfLength, f => f.Random.Int(300, 500))
            .RuleFor(o => o.AddressId, f => f.PickRandom(addressList).Id);
        return branchFaker.Generate(amount);
    }
}