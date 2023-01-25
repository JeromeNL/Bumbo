using Bogus;
using Bumbo.Data.Models;
using Bumbo.Data.Seeders.Interfaces;

namespace Bumbo.Data.Seeders.DataSeeders;

public abstract class OpeningHoursDataSeeder : IDataSeeder
{
    private static int Id { get; set; }

    public static IEnumerable<OpeningHours> GenerateOpeningHours(List<Branch> branches, string locale)
    {
        var openingHoursList = new List<OpeningHours>();
        // For each branch, each day of the week, generate opening hours
        foreach (var branch in branches)
        foreach (var day in Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>())
        {
            var openingHoursFaker = new Faker<OpeningHours>(locale)
                .RuleFor(o => o.Id, _ => ++Id)
                .RuleFor(o => o.BranchId, _ => branch.Id)
                .RuleFor(o => o.Day, _ => day)
                .RuleFor(o => o.OpeningTime, f => new TimeSpan(f.Random.Int(8, 9), 0, 0))
                .RuleFor(o => o.ClosingTime, f => new TimeSpan(f.Random.Int(16, 20), 0, 0));
            openingHoursList.Add(openingHoursFaker.Generate());
        }

        return openingHoursList;
    }
}