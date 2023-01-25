namespace Bumbo.Prognosis.Holiday;

public class EasterFirst : IHoliday
{
    public DateTime GetDateForThisYear(DateTime input)
    {
        return DateTime.Parse(GaussEaster(input.Year));
    }

    /// <summary>
    /// Uses Guass' algorithm to calculate easter for a year
    /// </summary>
    /// <param name="y">Year</param>
    /// <returns>yyyy-mm-dd for easter</returns>
    private static string GaussEaster(int y)
    {
        // All calculations done
        // on the basis of
        // Gauss Easter Algorithm
        float a = y % 19;
        float b = y % 4;
        float c = y % 7;
        float p = y / 100;
        float q = (int)((13 + 8 * p) / 25);
        float m = (int)(15 - q + p - (int)(p / 4)) % 30;
        float n = (int)(4 + p - (int)(p / 4)) % 7;
        var d = (19 * a + m) % 30;
        var e = (2 * b + 4 * c + 6 * d + n) % 7;
        var days = (int)(22 + d + e);

        switch (d)
        {
            // A corner case,
            // when D is 29
            case 29 when e == 6:
                return y + "-04"
                         + "-19";
            // Another corner case,
            // when D is 28
            case 28 when e == 6:
                return y + "-04"
                         + "-18";
        }

        // If days > 31, move to April
        // April = 4th Month
        if (days > 31)
        {
            return y + "-04-" + (days - 31);
        }

        // Otherwise, stay on March
        // March = 3rd Month

        return y + "-03-" + days;
    }
}