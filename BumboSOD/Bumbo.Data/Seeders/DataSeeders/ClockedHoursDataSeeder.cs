using Bogus;
using Bumbo.Data.Models;

namespace Bumbo.Data.Seeders.DataSeeders;

public abstract class ClockedHoursDataSeeder
{
    private static int Id { get; set; }

    public static IEnumerable<ClockedHours> GenerateClockedHours(List<Shift> shifts, string locale)
    {
        var clockedHoursList = new List<ClockedHours>();
        foreach (var shift in shifts)
        {
            if (shift.End > DateTime.Now) continue;

            var clockedHoursFaker = new Faker<ClockedHours>(locale)
                .RuleFor(o => o.Id, _ => ++Id)
                .RuleFor(o => o.EmployeeId, _ => shift.EmployeeId)
                .RuleFor(o => o.ClockedIn, f => shift.Start.AddMinutes(f.Random.Int(-10, 25)))
                .RuleFor(o => o.ClockedOut, f => shift.End.AddMinutes(f.Random.Int(-5, 60)));
            clockedHoursList.Add(clockedHoursFaker.Generate());
        }

        return clockedHoursList;
    }
}