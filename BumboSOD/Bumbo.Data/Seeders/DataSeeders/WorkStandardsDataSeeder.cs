using Bogus;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Bumbo.Data.Seeders.Interfaces;

namespace Bumbo.Data.Seeders.DataSeeders;

public abstract class WorkStandardsDataSeeder : IDataSeeder
{
    private static int Id { get; set; }

    public static IEnumerable<WorkStandards> GenerateWorkStandards(List<Branch> branches)
    {
        var workStandardsList = new List<WorkStandards>();

        foreach (var branch in branches)
            // Generate 2 past edit dates
            for (var i = 0; i < 2; i++)
            {
                var pastDate = new Faker().Date.Past();
                foreach (var workStandardType in Enum.GetValues(typeof(WorkStandardTypes)))
                {
                    var workStandardFaker = new Faker<WorkStandards>()
                        .RuleFor(o => o.Id, _ => ++Id)
                        .RuleFor(o => o.Task, _ => workStandardType)
                        .RuleFor(o => o.BranchId, _ => branch.Id)
                        .RuleFor(o => o.DateEntered, _ => pastDate)
                        .RuleFor(o => o.RequiredTimeInMinutes, f => f.Random.Decimal(0.2m, 5m));
                    workStandardsList.Add(workStandardFaker.Generate());
                }
            }

        return workStandardsList;
    }
}