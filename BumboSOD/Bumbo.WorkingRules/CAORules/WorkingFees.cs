using Bumbo.Data.Models.Enums;

namespace Bumbo.WorkingRules.CAORules;

public class WorkingFees
{
    public readonly Dictionary<Allowances, TimeSpan> WorkedHoursPerAllowance;

    public WorkingFees()
    {
        WorkedHoursPerAllowance = new Dictionary<Allowances, TimeSpan>
        {
            {
                Allowances.RegulierGekloktUur, new TimeSpan()
            },
            {
                Allowances.Tussen20UurEn21Uur, new TimeSpan()
            },
            {
                Allowances.TussenMiddernachtEn6Uur, new TimeSpan()
            },
            {
                Allowances.ZaterdagTussen18UurEnMidderNacht, new TimeSpan()
            },
            {
                Allowances.Zondag, new TimeSpan()
            },
            {
                Allowances.Vakantie, new TimeSpan()
            },
            {
                Allowances.Arbeidsongeschiktheid, new TimeSpan()
            }
        };
    }

    /// <summary>
    /// Add time to a specific allowance in WorkingFees
    /// </summary>
    /// <param name="allowance"></param>
    /// <param name="timeSpan"></param>
    public void AddTime(Allowances allowance, TimeSpan timeSpan)
    {
        if (!WorkedHoursPerAllowance.ContainsKey(allowance))
        {
            return;
        }

        WorkedHoursPerAllowance[allowance] += timeSpan;
    }

    /// <summary>
    /// Get the time of a specific allowance in WorkingFees
    /// </summary>
    /// <param name="allowance"></param>
    /// <returns>TimeSpan: total time of specific allowance</returns>
    public TimeSpan GetTime(Allowances allowance)
    {
        if (!WorkedHoursPerAllowance.ContainsKey(allowance))
        {
            return TimeSpan.Zero;
        }

        WorkedHoursPerAllowance.TryGetValue(allowance, out var hoursInAllowance);
        return hoursInAllowance;
    }

    /// <summary>
    /// Get the total amount of time with fees in WorkingFees
    /// </summary>
    /// <returns>TimeSpan: Total time with fees</returns>
    public TimeSpan GetTotalTimeWithFee()
    {
        var totalTime = new TimeSpan();
        foreach (var item in WorkedHoursPerAllowance)
        {
            totalTime += item.Value;
        }

        return totalTime;
    }
}