namespace Bumbo.Prognosis.Holiday;

public class KingsDay : IHoliday
{
    public DateTime GetDateForThisYear(DateTime input)
    {
        return DateTime.Parse(input.Year + "-04-27");
    }
}