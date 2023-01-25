using Bogus;
using Bumbo.Data.Models;
using Bumbo.Data.Seeders.Interfaces;
using Bumbo.Data.Utils;

namespace Bumbo.Data.Seeders.DataSeeders;

public abstract class ShiftDataSeeder : IDataSeeder
{
    private static int Id { get; set; }

    public static List<Shift> GenerateShifts(IEnumerable<ApplicationUser> employees, int startShifts, int endShifts, string locale)
    {
        var shiftList = new List<Shift>();
        foreach (var employee in employees)
        {
            if (new Random().Next(0, 10) > 5) continue;
            // Range of days to generate shifts for
            var startDays = DateTime.Now.AddDays(startShifts);
            var endDays = DateTime.Now.AddDays(endShifts);

            // For each day generate a shift
            foreach (var day in DateUtil.EachDay(startDays, endDays))
            {
                if (new Random().Next(0, 10) > 2) continue;
                var shiftDay = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);
                var shiftFaker = new Faker<Shift>(locale)
                    .RuleFor(o => o.Id, _ => ++Id)
                    .RuleFor(o => o.EmployeeId, _ => employee.Id)
                    .RuleFor(o => o.DepartmentId, _ => employee.Departments!.First().DepartmentId)
                    .RuleFor(o => o.Start, f => shiftDay.AddHours(f.Random.Int(9, 14)).AddMinutes(f.Random.Bool() ? 30 : 0))
                    .RuleFor(o => o.End, f => shiftDay.AddHours(f.Random.Int(18, 20)).AddMinutes(f.Random.Bool() ? 30 : 0))
                    .RuleFor(o => o.IsPublished, _ => true)
                    .RuleFor(o => o.IsIll, f => f.Random.Int(0, 10) == 0);
                shiftList.Add(shiftFaker.Generate());
            }
        }

        return shiftList;
    }
}