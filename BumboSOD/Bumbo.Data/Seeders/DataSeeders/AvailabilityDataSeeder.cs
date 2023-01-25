using Bogus;
using Bumbo.Data.Models;
using Bumbo.Data.Seeders.Interfaces;
using Bumbo.Data.Utils;

namespace Bumbo.Data.Seeders.DataSeeders;

public abstract class AvailabilityDataSeeder : IDataSeeder
{
    private static int Id { get; set; }

    public static IEnumerable<StandardAvailability> GenerateStandardAvailabilities(List<ApplicationUser> employeeList, string locale)
    {
        var standardAvailabilityList = new List<StandardAvailability>();

        foreach (var employee in employeeList)
        foreach (var dayOfWeek in Enum.GetValues<DayOfWeek>())
        {
            if (new Random().Next(0, 10) > 3) continue;
            var standardAvailabilityFaker = new Faker<StandardAvailability>(locale)
                .RuleFor(o => o.EmployeeId, _ => employee.Id)
                .RuleFor(o => o.DayOfWeek, _ => dayOfWeek)
                .RuleFor(o => o.Start, f => new DateTime(2000, 1, 1, f.Random.Int(9, 14), f.Random.Bool() ? 30 : 0, 0))
                .RuleFor(o => o.End, f => new DateTime(2000, 1, 1, f.Random.Int(16, 20), f.Random.Bool() ? 30 : 0, 0));
            standardAvailabilityList.Add(standardAvailabilityFaker.Generate());
        }

        return standardAvailabilityList;
    }

    public static IEnumerable<SpecialAvailability> GenerateSpecialAvailabilities(List<ApplicationUser> employeeList, string locale, int availabilities, int endShifts)
    {
        var availabilityList = new List<SpecialAvailability>();
        var dateStart = DateTime.Now.AddDays(endShifts);
        var dateEnd = DateTime.Now.AddDays(availabilities);
        foreach (var day in DateUtil.EachDay(dateStart, dateEnd))
        {
            if (new Random().Next(0, 10) > 2) continue;
            var availabilityDay = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);
            var availabilityFaker = new Faker<SpecialAvailability>(locale)
                .RuleFor(o => o.Id, _ => ++Id)
                .RuleFor(o => o.Start, f => availabilityDay.AddHours(f.Random.Int(7, 12)))
                .RuleFor(o => o.End, f => availabilityDay.AddHours(f.Random.Int(17, 22)));
            availabilityList.AddRange(employeeList
                .Select(employee => availabilityFaker
                    .RuleFor(o => o.EmployeeId, _ => employee.Id)
                    .Generate())
                .ToList());
        }

        return availabilityList;
    }
}