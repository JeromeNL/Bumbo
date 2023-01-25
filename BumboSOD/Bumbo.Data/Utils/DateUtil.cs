using System.Globalization;

namespace Bumbo.Data.Utils;

public static class DateUtil
{
    private const string _dateFormat = "dd-MM-yyyy";
    private const string _isoFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    private static readonly CultureInfo _cultureInfo = new("nl-NL");

    public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
    {
        for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
            yield return day;
    }

    public static int GetWeekNumberOfDateTime(DateTime dateTime)
    {
        if (dateTime.Date.DayOfWeek == DayOfWeek.Sunday)
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime.Date.AddDays(-7), CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        return _cultureInfo.Calendar.GetWeekOfYear(dateTime.Date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
    }

    public static DateTime DateTimeFromString(string dateTimeString)
    {
        return DateTime.ParseExact(dateTimeString, _dateFormat, _cultureInfo);
    }

    public static DateTime GetFirstDateOfWeek(int year, int weekOfYear)
    {
        var jan1 = new DateTime(year, 1, 1);

        var daysOffset = (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;

        var firstMonday = jan1.AddDays(daysOffset);
        var formattedFirstMonday = DateTime.Parse(firstMonday.ToString("dd-MM-yyyy"));

        var firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(jan1, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);

        if (firstWeek <= 1)
        {
            weekOfYear -= 1;
        }

        var amountOfDaysToAdd = (weekOfYear - GetWeekNumberOfDateTime(formattedFirstMonday)) * 7;
        return formattedFirstMonday.AddDays(amountOfDaysToAdd);
    }

    public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek)
    {
        var diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
        return dateTime.AddDays(-1 * diff).Date;
    }

    public static string ToIsoString(this DateTime dateTime)
    {
        return dateTime.ToUniversalTime().ToString(_isoFormat);
    }

    public static (DateTime first, DateTime last) GetFirstAndLastDayOfMonth(int month, int year)
    {
        // Get the first and last day of the current month
        var firstDay = new DateTime(year, month, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);

        return (firstDay, lastDay);
    }

    public static int CalculateAgeFromBirthDate(DateTime birthDate)
    {
        var age = DateTime.Today - birthDate;
        return age.Days / 365;
    }
}