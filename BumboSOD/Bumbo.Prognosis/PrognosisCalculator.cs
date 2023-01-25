using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Bumbo.Prognosis.Repositories;

namespace Bumbo.Prognosis;

/// <summary>
/// Calculates the prognosis for all departments for a given branch
/// </summary>
public class PrognosisCalculator
{
    private readonly DaysFinder _daysFinder;
    private readonly IPrognosisRepository _repo;

    public PrognosisCalculator(IPrognosisRepository repo)
    {
        _repo = repo;
        _daysFinder = new DaysFinder(_repo);
    }

    /// <summary>
    /// Calculates the required man hours for all departments
    /// </summary>
    /// <returns>List of prognoses for all departments</returns>
    public IDictionary<Department, Data.Models.Prognosis> CalculateManHoursForEntireBranchForGivenDay(DateTime calculationDate, int branchId)
    {
        // Five days are used for the prognosis calculation
        // - The four most recent days on the same weekday that have historical data available
        // AND
        // - If this day is close to a holiday, it uses the relative day to that holiday last year)
        // OR
        // - If the there is not a holiday nearby, it uses the day 365 days ago
        var calculationDays = _daysFinder.GetDaysForPrognosisCalculation(branchId, calculationDate);


        // Use the given day to make a prediction
        var predictedColli = CalculatePredictedAmountOfColli(calculationDays, branchId);
        var predictedCustomers = CalculatePredictedAmountOfCustomers(calculationDays, branchId);

        // Use prediction to calculate prognosis for each department:
        var departments = _repo.GetAllDepartments();

        // Key: department name
        // Value: prognosis for department
        var prognosis = departments
            .ToDictionary(
                department => department,
                department => CalculatePredictionForDepartment(calculationDate, department, predictedColli, predictedCustomers, branchId)
            );
        return prognosis;
    }

    ///<summary>
    /// Calculates the amount of working hours needed for the predicted amount of Colli and Customers for a specific department
    ///</summary>
    /// <returns>Prognosis for a specific Department</returns>
    private Data.Models.Prognosis CalculatePredictionForDepartment(
        DateTime calculationDate,
        Department department,
        decimal predictAmountOfColli,
        decimal predictAmountOfCustomers,
        int branchId
    )
    {
        var branchWithWorkStandard = _repo.BranchWithWorkStandard(branchId);

        // Validate if the calculation can be performed correctly
        if (branchWithWorkStandard?.WorkStandards?.Count < 1 ||
            predictAmountOfColli < 0 ||
            predictAmountOfCustomers < 0
           )
        {
            return new Data.Models.Prognosis
            {
                BranchId = branchId,
                Date = calculationDate,
                Department = department,
                DepartmentId = department.Id,
                ManHoursExpected = 0,
                CalculationSuccessful = false
            };
        }

        var workStandardsForDepartment = branchWithWorkStandard
            .WorkStandards
            .Where(ws => ws.RequiredTimeInMinutes > 0m)
            .OrderByDescending(ws => ws.DateEntered)
            .ToList();

        // Each department has it's own calculation
        var workHours = department.Name switch
        {
            "Kassa" =>
                predictAmountOfCustomers / (60m / workStandardsForDepartment.FirstOrDefault(e => e.Task == WorkStandardTypes.Kassa)!.RequiredTimeInMinutes),
            "Vers" => predictAmountOfCustomers / (60m / workStandardsForDepartment.FirstOrDefault(e => e.Task == WorkStandardTypes.Vers)!.RequiredTimeInMinutes),
            "Vakkenvullen" => predictAmountOfColli / (60m / workStandardsForDepartment.FirstOrDefault(e => e.Task == WorkStandardTypes.Uitladen)!.RequiredTimeInMinutes)
                              + predictAmountOfColli / (60m / workStandardsForDepartment.FirstOrDefault(e => e.Task == WorkStandardTypes.VakkenVullen)!.RequiredTimeInMinutes)
                              + branchWithWorkStandard.ShelfLength / (60m / workStandardsForDepartment.FirstOrDefault(e => e.Task == WorkStandardTypes.Spiegelen)!.RequiredTimeInMinutes),
            _ => -1m
        };

        return new Data.Models.Prognosis
        {
            Branch = branchWithWorkStandard,
            BranchId = branchWithWorkStandard.Id,
            Date = calculationDate,
            Department = department,
            DepartmentId = department.Id,
            ManHoursExpected = workHours,
            CalculationSuccessful = workHours > 0 // If workhours < 1
        };
    }

    #region Colli Calculation

    ///<summary>
    /// Uses the four most recent weeks of HistoricalData,
    /// and last year to calculate the predicted amount of Colli
    ///</summary>
    /// <returns>Amount of Colli predicted.</returns>
    private decimal CalculatePredictedAmountOfColli(PrognosisDaysDto prognosisDaysDto, int branchId)
    {
        var avgColli = GetAverageColliFromPastWeekdaysForBranch(prognosisDaysDto.SameWeekDayForPreviousFourWeeks, branchId);
        var colliLastYear = GetAmountOfColliFromLastYearForBranch(prognosisDaysDto, branchId);

        if (avgColli < 0 && colliLastYear < 0)
        {
            return -1;
        }

        if (colliLastYear < 0)
        {
            return avgColli;
        }

        return avgColli *
               0.25m +
               colliLastYear * 0.75m;
    }

    /// <summary>
    /// Calculates the average amount of colli from the past 4 specific weekdays
    /// Example: Day to schedule: Monday 7-11-22 gives averages of Days: 31-10, 24-10, 17-10, 10-10
    /// </summary>
    ///  <param name="datesOfFourDays"></param>
    /// <param name="branchId"></param>
    /// <returns>Average amount of colli from the past four weekdays</returns>
    private decimal GetAverageColliFromPastWeekdaysForBranch(IReadOnlyList<DateTime> datesOfFourDays,
        int branchId)
    {
        if (datesOfFourDays.Count == 0)
        {
            return -1;
        }

        var amountOfColli = new decimal[datesOfFourDays.Count];

        for (var i = 0; i < datesOfFourDays.Count; i++)
        {
            var currentDate = datesOfFourDays[i];

            var historicalData = _repo.FindHistoricalDataForThisDate(currentDate, branchId);

            if (historicalData is null)
            {
                return -1;
            }

            amountOfColli[i] = historicalData.AmountColi;
        }

        return amountOfColli.Average();
    }

    /// <summary>
    ///  Get the amount of Colli from the same day last year
    ///  Holidays are taken into account: Easter is every year on a different date
    /// </summary>
    /// <param name="input">DTO with necessary data</param>
    /// <param name="branchId"></param>
    /// <returns>Amount of colli from past year</returns>
    private decimal GetAmountOfColliFromLastYearForBranch(PrognosisDaysDto input, int branchId)
    {
        var historicalData = _repo.FindHistoricalDataForThisDate(input.TodayLastYearRelativeToHoliday, branchId);

        return historicalData?.AmountColi ?? -1;
    }

    #endregion

    #region Customers Calculation

    /// <summary>
    /// Uses the four most recent weeks of HistoricalData,
    /// and the same day last year to calculate the predicted amount of customers
    /// </summary>
    /// <returns>Amount of Customers predicted.</returns>
    private decimal CalculatePredictedAmountOfCustomers(PrognosisDaysDto prognosisDaysDto,
        int branchId)
    {
        var avgCustomer = GetAverageCustomersPastWeekdaysForBranch(prognosisDaysDto.SameWeekDayForPreviousFourWeeks, branchId);
        var customerLastYear = GetAmountOfCustomersFromLastYearForBranch(prognosisDaysDto, branchId);


        if (avgCustomer < 0 && customerLastYear < 0)
        {
            return -1;
        }

        if (customerLastYear < 0)
        {
            return avgCustomer;
        }

        return avgCustomer *
               0.25m +
               customerLastYear * 0.75m;
    }

    /// <summary>
    ///  Calculate the average amount of Customers from the past 4 specific days
    ///  Example: Day to schedule: Monday 7-11-22 - Days: 31-10, 24-10, 17-10, 10-10
    /// </summary>
    ///  <param name="datesOfFourDays"></param>
    /// <param name="branchId"></param>
    /// <returns>Average amount of customers from the past four weekdays</returns>
    private decimal GetAverageCustomersPastWeekdaysForBranch(IReadOnlyList<DateTime> datesOfFourDays,
        int branchId)
    {
        var amountOfCustomers = new decimal[datesOfFourDays.Count];

        if (datesOfFourDays.Count == 0)
        {
            return -1;
        }

        for (var i = 0; i < datesOfFourDays.Count; i++)
        {
            var currentDate = datesOfFourDays[i];

            var historicalData = _repo.FindHistoricalDataForThisDate(currentDate, branchId);

            if (historicalData is null)
            {
                return -1;
            }

            amountOfCustomers[i] = historicalData.AmountCustomers;
        }

        return amountOfCustomers.Average();
    }

    /// <summary>
    ///  Get the amount of Customers from the same day last year
    ///  Holidays are taken into account: Easter is every year on a different date
    /// </summary>
    /// <param name="input"></param>
    /// <param name="branchId"></param>
    /// <returns>Amount of Customers from past year</returns>
    private decimal GetAmountOfCustomersFromLastYearForBranch(PrognosisDaysDto input, int branchId)
    {
        var historicalData = _repo.FindHistoricalDataForThisDate(input.TodayLastYearRelativeToHoliday, branchId);

        return historicalData?.AmountCustomers ?? -1;
    }

    #endregion
}