using Bogus;
using Bumbo.Data.Models;
using Bumbo.Data.Utils;
using Microsoft.AspNetCore.Identity;

namespace Bumbo.Data.Seeders.DataSeeders;

public abstract class EmployeeDataSeeder
{
    public static (List<ApplicationUser> employees, List<ApplicationUserDepartment> linkTable, List<IdentityUserRole<string>> userRoles) GenerateEmployees(int amount, List<Department> departmentsList, IReadOnlyList<Address> addressList, List<Branch> branches, List<IdentityRole> identityRoles, string locale, string managerEmail, string defaultPassword)
    {
        var addressId = addressList.Count;
        var employeeList = new List<ApplicationUser>();
        var linkTable = new List<ApplicationUserDepartment>();
        string[] availableMiddleNames = { "van", "de", "van der", "van de" };

        var results = 0;

        foreach (var branch in branches)
        foreach (var department in departmentsList)
        {
            var employeeFaker = new Faker<ApplicationUser>(locale)
                .RuleFor(o => o.FirstName, f => f.Name.FirstName())
                .RuleFor(o => o.MiddleName, f => f.PickRandom(availableMiddleNames).OrNull(f))
                .RuleFor(o => o.LastName, f => f.Name.LastName())
                .RuleFor(o => o.PhoneNumber, f => f.Person.Phone)
                .RuleFor(o => o.Email, f => results++ == 0 ? managerEmail : f.Person.Email)
                .RuleFor(o => o.EmailConfirmed, _ => true)
                .RuleFor(o => o.RegistrationDate, f => f.Date.Past())
                .RuleFor(o => o.AddressId, _ => addressList[--addressId].Id)
                .RuleFor(o => o.BranchId, _ => branch.Id)
                .RuleFor(o => o.BirthDate, f => f.Date.Past(25, DateTime.Now.AddYears(-15)))
                .RuleFor(o => o.Id, f => f.Random.Guid().ToString());


            var employees = employeeFaker.Generate(amount);
            foreach (var employee in employees)
            {
                employee.UserName = employee.FirstName + employee.LastName + "_" + employee.Id[..5];
                employee.NormalizedEmail = employee.Email.ToUpper();
                employee.NormalizedUserName = employee.UserName.ToUpper();
                employee.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(employee, defaultPassword);
                employee.PayoutScale = GeneratePayoutScale(employee.BirthDate);

                var applicationUserDepartment = new ApplicationUserDepartment
                {
                    DepartmentId = department.Id,
                    EmployeeId = employee.Id
                };
                employee.Departments = new List<ApplicationUserDepartment> { applicationUserDepartment };
                linkTable.Add(applicationUserDepartment);
            }

            employeeList.AddRange(employees);
        }

        var userRoles = new List<IdentityUserRole<string>>();
        for (var i = 0; i < employeeList.Count; i++)
        {
            if (i == 0) userRoles.Add(new IdentityUserRole<string> { RoleId = identityRoles[1].Id, UserId = employeeList[i].Id });

            userRoles.Add(new IdentityUserRole<string> { RoleId = identityRoles[2].Id, UserId = employeeList[i].Id });
        }

        return (employeeList, linkTable, userRoles);
    }

    private static int GeneratePayoutScale(DateTime employeeBirthDate)
    {
        var age = DateUtil.CalculateAgeFromBirthDate(employeeBirthDate);
        return age switch
        {
            < 16 => 1,
            < 18 => 2,
            < 20 => 3,
            < 22 => 4,
            < 24 => 5,
            < 26 => 6,
            < 28 => 7,
            < 32 => 8,
            < 36 => 9,
            _ => 10
        };
    }
}