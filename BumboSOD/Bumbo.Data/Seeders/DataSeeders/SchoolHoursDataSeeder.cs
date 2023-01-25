using Bogus;
using Bumbo.Data.Models;
using Bumbo.Data.Seeders.Interfaces;

namespace Bumbo.Data.Seeders.DataSeeders;

public abstract class SchoolHoursDataSeeder : IDataSeeder
{
    private static int Id { get; set; }

    public static IEnumerable<StandardSchoolHours> GenerateStandardSchoolHours(List<ApplicationUser> employees, string locale)
    {
        var schoolHourList = new List<StandardSchoolHours>();

        foreach (var employee in employees)
        {
            var zeroTime = new DateTime(1, 1, 1);
            var timeSpan = DateTime.Now - employee.BirthDate;
            var years = (zeroTime + timeSpan).Year - 1;
            if (years >= 18) continue;

            foreach (var dayOfWeek in Enum.GetValues<DayOfWeek>())
            {
                if (new Random().Next(0, 7) >= 5) continue;
                var standardSchoolHourFaker = new Faker<StandardSchoolHours>(locale)
                    .RuleFor(o => o.EmployeeId, _ => employee.Id)
                    .RuleFor(o => o.DayOfWeek, _ => dayOfWeek)
                    .RuleFor(o => o.Hours, f => f.Random.Int(0, 10));
                schoolHourList.Add(standardSchoolHourFaker.Generate());
            }
        }

        return schoolHourList;
    }

    public static IEnumerable<SpecialSchoolHours> GenerateSpecialSchoolHours(List<ApplicationUser> employees, string locale)
    {
        var schoolHourList = new List<SpecialSchoolHours>();

        foreach (var employee in employees)
        {
            var zeroTime = new DateTime(1, 1, 1);
            var timeSpan = DateTime.Now - employee.BirthDate;
            var years = (zeroTime + timeSpan).Year - 1;
            if (years >= 18) continue;

            for (var i = 0; i < new Random().Next(0, 5); i++)
            {
                if (new Random().Next(0, 1) == 1) continue;

                var date = new Faker().Date.Between(DateTime.Now.AddDays(-30), DateTime.Now.AddDays(30));
                var specialSchoolhourFaker = new Faker<SpecialSchoolHours>(locale)
                    .RuleFor(o => o.Id, _ => ++Id)
                    .RuleFor(o => o.EmployeeId, _ => employee.Id)
                    .RuleFor(o => o.Start, _ => date)
                    .RuleFor(o => o.Hours, f => f.Random.Int(0, 10));
                schoolHourList.Add(specialSchoolhourFaker.Generate());
            }
        }

        return schoolHourList;
    }
}