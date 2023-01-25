namespace Bumbo.Prognosis.Holiday;

public class ChristmasFirst : IHoliday
{
    public DateTime GetDateForThisYear(DateTime input)
    {
        return DateTime.Parse(input.Year + "-12-25");
    }
}