using Bogus;
using Bumbo.Data.Models;
using Bumbo.Data.Seeders.Interfaces;
using Bumbo.Data.Utils;

namespace Bumbo.Data.Seeders.DataSeeders;

public abstract class HistoricalDataSeeder : IDataSeeder
{
    private static int Id { get; set; }

    public static IEnumerable<HistoricalData> GenerateHistoricalData(List<Branch> branches)
    {
        var historicalDataList = new List<HistoricalData>();

        foreach (var branch in branches)
        {
            var startDate = DateTime.Now.AddYears(-1);
            var endDate = DateTime.Now.AddDays(-1);

            foreach (var date in DateUtil.EachDay(startDate, endDate))
            {
                var historicalDataFaker = new Faker<HistoricalData>()
                    .RuleFor(o => o.Id, _ => ++Id)
                    .RuleFor(o => o.BranchId, _ => branch.Id)
                    .RuleFor(o => o.Date, _ => date)
                    .RuleFor(o => o.AmountCustomers, f => f.Random.Int(600, 1600))
                    .RuleFor(o => o.AmountColi, f => f.Random.Int(10, 95));
                historicalDataList.Add(historicalDataFaker.Generate());
            }
        }

        return historicalDataList;
    }
}