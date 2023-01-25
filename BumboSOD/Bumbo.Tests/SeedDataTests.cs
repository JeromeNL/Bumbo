namespace Bumbo.Tests;

public class SeedDataTests
{
    private const int _amountOfEmployees = 2;
    private const int _amountOfBranches = 2;
    private static readonly string[] _availableDepartmentsList = { "Kassa", "Vers" };
    private static readonly int _amountOfAddresses = _amountOfBranches * _availableDepartmentsList.Length * _amountOfEmployees;

    /*[Fact]
    public void InitializeSeedData_AllModelsHaveData()
    {
        var addressList = DataSeeder.GenerateAddresses(_amountOfAddresses);
        Assert.True(addressList.Count > 0, "The addressList was empty");

        var departmentsList = DataSeeder.GenerateDepartments(_availableDepartmentsList);
        Assert.True(departmentsList.Count > 0, "The departmentsList was empty");

        var branchList = DataSeeder.GenerateBranches(_amountOfBranches, addressList);
        Assert.True(branchList.Count > 0, "The branchList was empty");
        Assert.True(branchList.Count == _amountOfBranches, "Incorrect amount of branches generated");

        var employeeList = DataSeeder.GenerateEmployees(_amountOfEmployees, departmentsList, addressList, branchList, new List<IdentityRole>()).employees;
        Assert.True(employeeList.Count > 0, "The employeeList was empty");

        var availabilityList = DataSeeder.GenerateAvailabilities(employeeList);
        Assert.True(availabilityList.Count > 0, "The availabilityList was empty");

        var openingHoursList = DataSeeder.GenerateOpeningHours(branchList);
        Assert.True(openingHoursList.Count > 0, "The openingHoursList was empty");

        var shiftList = DataSeeder.GenerateShifts(employeeList);
        Assert.True(shiftList.Count > 0, "The shiftList was empty");

        var payoutList = DataSeeder.GeneratePayouts(shiftList);
        Assert.True(payoutList.Count > 0, "The payoutList was empty");

        var exchangeRequestsList = DataSeeder.GenerateExchangeRequests(shiftList);
        Assert.True(exchangeRequestsList.Count > 0, "The exchangeRequestsList was empty");

        var schoolHoursList = DataSeeder.GenerateSchoolHours(employeeList);
        Assert.True(schoolHoursList.Count > 0, "The schoolHoursList was empty");

        var clockedHoursList = DataSeeder.GenerateClockedHours(employeeList);
        Assert.True(clockedHoursList.Count > 0, "The clockedHoursList was empty");
    }*/
}