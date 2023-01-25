using System.Runtime.CompilerServices;
using Bumbo.Prognosis.Holiday;
using Bumbo.Prognosis.Repositories;

[assembly: InternalsVisibleTo("Bumbo.Tests")]

namespace Bumbo.Prognosis;

public class DaysFinder
{
    private const int _daysUntilHolidayShouldBeAccountedFor = 5;
    private readonly IPrognosisRepository _repo;

    public DaysFinder(IPrognosisRepository repo)
    {
        _repo = repo;
    }

    ///  <summary>
    ///  Finds the necessary days needed to calculate the prognosis
    /// </summary>
    ///  <param name="branchId">The Id for which the historical data will be used</param>
    ///  <param name="input">The input day for which the prognosis should be calculated</param>
    ///  <returns>
    ///  DTO with the necessary days for the calculation of the prognosis for the input day
    ///  </returns>
    public PrognosisDaysDto GetDaysForPrognosisCalculation(int branchId, DateTime input)
    {
        var lastFourSameWeekdays = GetLastFourSameWeekdays(input, branchId);
        var todayLastYearRelativeToHoliday = RelativeHolidayDateLastYearIfWithinRange(input);

        return new PrognosisDaysDto(
            SameWeekDayForPreviousFourWeeks: lastFourSameWeekdays,
            TodayLastYearRelativeToHoliday: todayLastYearRelativeToHoliday
        );
    }

    /// <summary>
    /// Finds for given branch the most four most recent days in HistoricalData on the same weekday
    /// </summary>
    /// <param name="input"></param>
    /// <param name="branchId"></param>
    /// <returns>
    /// Four (or less if there's not enough historical data)
    /// equal weekdays 7, 14, 21 and 28 days ago from the given date.
    /// </returns>
    private List<DateTime> GetLastFourSameWeekdays(DateTime input, int branchId)
    {
        var sameWeekDays = new List<DateTime>();

        // Only return dates for which historical data for this branch is available
        var historicalDataForBranch = _repo.GetHistoricalDataDescending(branchId);

        foreach (var item in historicalDataForBranch)
        {
            if (sameWeekDays.Count > 3) break;

            // Only save dates from the past, with the same weekday as input
            if (item.Date >= input || item.Date.DayOfWeek != input.DayOfWeek) continue;

            sameWeekDays.Add(item.Date);
        }

        return sameWeekDays;
    }

    /// <summary>
    /// Finds the same day last year relative to a
    /// holiday if it's within the margin of DaysUntilHolidaysShouldBeAccountedFor
    /// </summary>
    /// <param name="inputDate">The date that that will be looked for last year</param>
    /// <returns>
    /// The input date last year (or relative to a holiday, if it's close enough)
    /// </returns>
    private static DateTime RelativeHolidayDateLastYearIfWithinRange(DateTime inputDate)
    {
        var nextHoliday = FindNextPublicHoliday(inputDate);

        var daysUntilNextHoliday = nextHoliday.GetDateForThisYear(inputDate).Subtract(inputDate).Days;

        var holidayWithinRange = daysUntilNextHoliday <= _daysUntilHolidayShouldBeAccountedFor;
        if (!holidayWithinRange) return inputDate.AddYears(-1);

        var sameHolidayLastYear = nextHoliday.GetDateForThisYear(inputDate.AddYears(-1));

        return sameHolidayLastYear.AddDays(-daysUntilNextHoliday);
    }

    /// <summary>
    /// Gets the closest next holiday for the given date
    /// </summary>
    /// <param name="inputDate">From date</param>
    /// <returns>Date of the next holiday</returns>
    private static IHoliday FindNextPublicHoliday(DateTime inputDate)
    {
        var holidays = GetAllHolidaysForThisYear();

        TimeSpan? timeSpan = null;

        // Find the closest holiday to the inputDate
        // Return the closest future holiday
        var iHoliday = holidays.First();

        foreach (var holiday in holidays)
        {
            var holidayDate = holiday.GetDateForThisYear(inputDate);

            var diff = holidayDate.Subtract(inputDate);

            // Only use days in the future
            if (diff.Days < 0)
            {
                continue;
            }

            // If this is the first valid holiday, update the values and continue
            if (!timeSpan.HasValue)
            {
                timeSpan = diff;
                iHoliday = holiday;
                continue;
            }

            // Only update if holiday is closer
            if (diff.Days >= timeSpan.Value.Days) continue;

            timeSpan = diff;
            iHoliday = holiday;
        }

        return iHoliday;
    }

    /// <summary>
    /// List of all holidays that are taken account for
    /// Register here all IHoliday implementations
    /// </summary>
    /// <returns></returns>
    public static IList<IHoliday> GetAllHolidaysForThisYear()
    {
        return new List<IHoliday>
        {
            new EasterFirst(),
            new EasterSecond(),
            new ChristmasFirst(),
            new ChristmasSecond(),
            new KingsDay(),
            new NewYearsEve()
        };
    }
}