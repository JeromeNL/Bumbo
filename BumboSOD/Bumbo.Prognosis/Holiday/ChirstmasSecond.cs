namespace Bumbo.Prognosis.Holiday;

public class ChristmasSecond : IHoliday
{
    public DateTime GetDateForThisYear(DateTime input)
    {
        return DateTime.Parse(input.Year + "-12-26");
    }
}