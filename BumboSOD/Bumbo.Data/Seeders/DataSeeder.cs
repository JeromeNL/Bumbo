using Bogus;
using Bumbo.Data.Models;
using Bumbo.Data.Seeders.DataSeeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bumbo.Data.Seeders;

public static class DataSeeder
{
    private const int _seed = 181079;
    private const string _locale = "nl";
    private const int _startShifts = -40; // Amount of days before now
    private const int _endShifts = 10; // Amount of days after now
    private const int _availabilities = _endShifts + 10; // Days after endShifts

    private const int _amountOfEmployeesPerBranchPerDepartment = 10;
    private const int _amountOfBranches = 2;

    // Default accounts
    private const string _managerEmail = "manager@bumbo.nl";
    private const string _defaultPassword = "bumbo";
    private static readonly string[] _availableDepartmentsList = { "Kassa", "Vers", "Vakkenvullen" };
    private static readonly int _amountOfAddresses = _amountOfBranches * _availableDepartmentsList.Length * _amountOfEmployeesPerBranchPerDepartment;

    public static void InitializeSeedData(this ModelBuilder modelBuilder)
    {
        Randomizer.Seed = new Random(_seed);
        var identityRoles = modelBuilder.SeedRoles();
        // Generate address for each
        var addressList = AddressDataSeeder.GenerateAddresses(_amountOfAddresses, _locale);
        var departmentsList = DepartmentDataSeeder.GenerateDepartments(_availableDepartmentsList, _locale);
        var branchList = BranchDataSeeder.GenerateBranches(_amountOfBranches, addressList, _locale);
        var employeeAndLinkList = EmployeeDataSeeder.GenerateEmployees(_amountOfEmployeesPerBranchPerDepartment, departmentsList, addressList, branchList, identityRoles, _locale, _managerEmail, _defaultPassword);
        var employeeList = employeeAndLinkList.employees;
        var standardAvailabilityList = AvailabilityDataSeeder.GenerateStandardAvailabilities(employeeList, _locale);
        var specialAvailabilityList = AvailabilityDataSeeder.GenerateSpecialAvailabilities(employeeList, _locale, _availabilities, _endShifts);
        var openingHoursList = OpeningHoursDataSeeder.GenerateOpeningHours(branchList, _locale);
        var shiftList = ShiftDataSeeder.GenerateShifts(employeeList, _startShifts, _endShifts, _locale);
        var payoutList = PayoutsDataSeeder.GeneratePayouts(shiftList, _locale);
        var exchangeRequestsList = ExchangeRequestDataSeeder.GenerateExchangeRequests(shiftList, _amountOfBranches, _locale);
        var standardSchoolHoursList = SchoolHoursDataSeeder.GenerateStandardSchoolHours(employeeList, _locale);
        var specialSchoolHoursList = SchoolHoursDataSeeder.GenerateSpecialSchoolHours(employeeList, _locale);
        var workStandardsList = WorkStandardsDataSeeder.GenerateWorkStandards(branchList);
        var prognosisList = HistoricalDataSeeder.GenerateHistoricalData(branchList);
        var clockedHoursList = ClockedHoursDataSeeder.GenerateClockedHours(shiftList, _locale);
        var notificationsList = NotificationDataSeeder.GenerateNotifications(employeeList);

        // Seed data cannot contain objects, department was temporarily set for generating shifts
        foreach (var employee in employeeList)
            employee.Departments = null;

        modelBuilder.Entity<Address>().HasData(addressList);
        modelBuilder.Entity<Department>().HasData(departmentsList);
        modelBuilder.Entity<Branch>().HasData(branchList);
        modelBuilder.Entity<ApplicationUser>().HasData(employeeList);
        modelBuilder.Entity<StandardAvailability>().HasData(standardAvailabilityList);
        modelBuilder.Entity<SpecialAvailability>().HasData(specialAvailabilityList);
        modelBuilder.Entity<OpeningHours>().HasData(openingHoursList);
        modelBuilder.Entity<Shift>().HasData(shiftList);
        modelBuilder.Entity<Payout>().HasData(payoutList);
        modelBuilder.Entity<ExchangeRequest>().HasData(exchangeRequestsList);
        modelBuilder.Entity<StandardSchoolHours>().HasData(standardSchoolHoursList);
        modelBuilder.Entity<SpecialSchoolHours>().HasData(specialSchoolHoursList);
        modelBuilder.Entity<WorkStandards>().HasData(workStandardsList);
        modelBuilder.Entity<HistoricalData>().HasData(prognosisList);
        modelBuilder.Entity<ClockedHours>().HasData(clockedHoursList);
        modelBuilder.Entity<ApplicationUserDepartment>().HasData(employeeAndLinkList.linkTable);
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(employeeAndLinkList.userRoles);
        modelBuilder.Entity<Notification>().HasData(notificationsList);
    }
}