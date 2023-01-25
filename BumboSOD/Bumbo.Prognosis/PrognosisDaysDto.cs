namespace Bumbo.Prognosis;

/// <summary>
/// Data transfer object with the necessary dates to calculate the prognosis
/// </summary>
/// <param name="SameWeekDayForPreviousFourWeeks">
/// Given the date, contains the last previous four equal weekdays
/// In case of monday, contains the last four mondays
/// </param>
/// <param name="TodayLastYearRelativeToHoliday">
/// If given day is within DaysFinder.DaysUntilHolidaysShouldBeAccountedFor days of holiday,
/// contains the same day last year relative to the same holiday
/// </param>
public record PrognosisDaysDto(
    List<DateTime> SameWeekDayForPreviousFourWeeks,
    DateTime TodayLastYearRelativeToHoliday
);