using Bogus;
using Bumbo.Data.Models;
using Bumbo.Data.Seeders.Interfaces;

namespace Bumbo.Data.Seeders.DataSeeders;

public abstract class DepartmentDataSeeder : IDataSeeder
{
    private static int Id { get; set; }

    public static List<Department> GenerateDepartments(IEnumerable<string> availableDepartments, string locale)
    {
        var departmentFaker = new Faker<Department>(locale)
            .RuleFor(o => o.Id, _ => ++Id);
        var departmentsList = availableDepartments.Select(department => departmentFaker.RuleFor(o => o.Name, _ => department).Generate()).ToList();
        return departmentsList;
    }
}