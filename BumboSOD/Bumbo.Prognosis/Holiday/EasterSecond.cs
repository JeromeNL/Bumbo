namespace Bumbo.Prognosis.Holiday;

public class EasterSecond : IHoliday
{
    public DateTime GetDateForThisYear(DateTime input)
    {
        EasterFirst easterFirst = new();
        var lastYearEasterFirst = easterFirst.GetDateForThisYear(input);

        return lastYearEasterFirst.AddDays(1);
    }
}