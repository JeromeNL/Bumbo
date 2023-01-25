namespace Bumbo.Prognosis.Holiday;

public class NewYearsEve : IHoliday
{
    public DateTime GetDateForThisYear(DateTime input)
    {
        return DateTime.Parse(input.Year + "-12-31");
    }
}