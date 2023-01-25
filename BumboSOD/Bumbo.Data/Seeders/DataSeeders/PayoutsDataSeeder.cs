using Bogus;
using Bumbo.Data.Models;
using Bumbo.Data.Seeders.Interfaces;
using Bumbo.Data.Utils;

namespace Bumbo.Data.Seeders.DataSeeders;

public abstract class PayoutsDataSeeder : IDataSeeder
{
    private static int Id { get; set; }

    public static IEnumerable<Payout> GeneratePayouts(IEnumerable<Shift> shifts, string locale)
    {
        var payoutList = new List<Payout>();
        var now = DateTime.Now;
        var pastShifts = shifts.Where(t => t.Start.CompareTo(now) < 0);
        foreach (var shift in pastShifts)
        {
            var endStartDifference = shift.End - shift.Start;
            var payoutFaker = new Faker<Payout>(locale)
                .RuleFor(o => o.Id, _ => ++Id)
                .RuleFor(o => o.EmployeeId, _ => shift.EmployeeId)
                .RuleFor(o => o.WeekNumber, _ => DateUtil.GetWeekNumberOfDateTime(shift.Start))
                .RuleFor(o => o.PayedHours, _ => (decimal)(endStartDifference.TotalHours + endStartDifference.TotalMinutes / 60))
                .RuleFor(o => o.Bonus, f => f.Random.Int(0, 10) == 0 ? 20m : 0m); // 10% chance of getting a bonus
            payoutList.Add(payoutFaker.Generate());
        }

        return payoutList;
    }
}