using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Bumbo.Prognosis;
using Bumbo.WorkingRules.Repositories;

namespace Bumbo.WorkingRules.CAORules;

public class WorkingFeesCalculator : IWorkingFeesCalculator
{
    private readonly ICaoRepository _repo;

    public WorkingFeesCalculator(ICaoRepository iCaoPrognosis)
    {
        _repo = iCaoPrognosis;
    }

    /// <summary>
    /// Calculate all the allowances for all the workedHours (and 'ill shifts') for all employees of a specific branch.
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <param name="branch"></param>
    /// <returns>Dictionary <key>: Employee <Value>: WorkingFees (amount of hours per allowance)</returns>
    public async Task<Dictionary<ApplicationUser, WorkingFees>> GetAllWorkedHoursWithAllowancesForPeriod(DateTime startTime, DateTime endTime, Branch branch)
    {
        var allFeesForEmployee = new Dictionary<ApplicationUser, WorkingFees>();
        var allShifts = await _repo.GetAllShiftsWithinPeriod(startTime, endTime, branch.Id);
        // add all SickLeave Allowances based on planned
        foreach (var shift in allShifts)
        {
            if (shift.IsIll)
            {
                if (allFeesForEmployee.ContainsKey(shift.Employee))
                {
                    var workingFeesOfEmployee = allFeesForEmployee[shift.Employee];
                    workingFeesOfEmployee.AddTime(Allowances.Arbeidsongeschiktheid, await GetDurationOfSickAllowanceForPeriod(shift.Employee, shift.Start, shift.End));
                    allFeesForEmployee[shift.Employee] = workingFeesOfEmployee;
                }
                else
                {
                    var workingFees = new WorkingFees();
                    workingFees.AddTime(Allowances.Arbeidsongeschiktheid, await GetDurationOfSickAllowanceForPeriod(shift.Employee, shift.Start, shift.End));
                    allFeesForEmployee.Add(shift.Employee, workingFees);
                }
            }
        }

        // add all worked hours based on 'WorkedHours' from the given period.
        var allWorkedHoursShifts = await _repo.GetAllWorkedHoursWithinPeriod(startTime, endTime, branch);
        // Only use worked hours that are approved by the manager
        allWorkedHoursShifts = allWorkedHoursShifts.Where(wh => wh.IsApproved).ToList();
        foreach (var workedHoursShift in allWorkedHoursShifts)
        {
            if (workedHoursShift.ClockedOut != null || workedHoursShift.ClockedOut != DateTime.MinValue)
            {
                var allowancesForWorkedShift = GetAllowancesForWorkedShift(workedHoursShift);
                var workingFeesOfEmployee = allFeesForEmployee.ContainsKey(workedHoursShift.Employee!) ? allFeesForEmployee[workedHoursShift.Employee!] : new WorkingFees();

                workingFeesOfEmployee.AddTime(Allowances.Zondag, allowancesForWorkedShift.GetTime(Allowances.Zondag));
                workingFeesOfEmployee.AddTime(Allowances.Vakantie, allowancesForWorkedShift.GetTime(Allowances.Vakantie));
                workingFeesOfEmployee.AddTime(Allowances.ZaterdagTussen18UurEnMidderNacht, allowancesForWorkedShift.GetTime(Allowances.ZaterdagTussen18UurEnMidderNacht));
                workingFeesOfEmployee.AddTime(Allowances.TussenMiddernachtEn6Uur, allowancesForWorkedShift.GetTime(Allowances.TussenMiddernachtEn6Uur));
                workingFeesOfEmployee.AddTime(Allowances.Tussen20UurEn21Uur, allowancesForWorkedShift.GetTime(Allowances.Tussen20UurEn21Uur));
                workingFeesOfEmployee.AddTime(Allowances.RegulierGekloktUur, allowancesForWorkedShift.GetTime(Allowances.RegulierGekloktUur));
                allFeesForEmployee[workedHoursShift.Employee!] = workingFeesOfEmployee;
            }
        }

        return allFeesForEmployee;
    }

    /// <summary>
    /// Calculate all the allowances for all the workedHours (and 'ill shifts') for a specific employee.
    /// </summary>
    /// <param name="workedHours"></param>
    /// <returns>WorkingFees: amount of hours per allowance</returns>
    public WorkingFees GetAllowancesForWorkedShift(ClockedHours workedHours)
    {
        if (workedHours.ClockedOut == null)
        {
            return null;
        }

        var shiftWorkedHours = new WorkingFees();

        shiftWorkedHours.AddTime(Allowances.Zondag, CalculateSundayAllowance(workedHours));
        shiftWorkedHours.AddTime(Allowances.Vakantie, CalculateHolidayAllowance(workedHours));
        shiftWorkedHours.AddTime(Allowances.ZaterdagTussen18UurEnMidderNacht, CalculateSaturday18Till24Allowance(workedHours));
        shiftWorkedHours.AddTime(Allowances.TussenMiddernachtEn6Uur, CalculateNightAllowance(workedHours));
        shiftWorkedHours.AddTime(Allowances.Tussen20UurEn21Uur, CalculateTimeBetween20And21Allowance(workedHours));
        shiftWorkedHours.AddTime(Allowances.RegulierGekloktUur, CalculateTimeDefault(workedHours, shiftWorkedHours.GetTotalTimeWithFee()));

        return shiftWorkedHours;
    }

    /// <summary>
    /// Calculates the 'sick leave allowance' for a specific employee for a given time range
    /// </summary>
    /// <param name="employee"></param>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <returns>TimeSpan: Amount of 'sick leave' hours for given time range</returns>
    public async Task<TimeSpan> GetDurationOfSickAllowanceForPeriod(ApplicationUser employee, DateTime startTime, DateTime endTime)
    {
        var shiftsInRange = new List<Shift>();
        for (var day = startTime.Date; day.Date <= endTime.Date; day = day.AddDays(1))
        {
            var item = await _repo.GetShiftsForDay(startTime.Date, employee.Id);
            shiftsInRange.AddRange(item.ToList().Where(e => e.IsIll));
        }

        var sickTimeCounter = new TimeSpan();
        foreach (var item in shiftsInRange)
        {
            sickTimeCounter += item.End - item.Start;
        }

        return sickTimeCounter;
    }

    /// <summary>
    /// Calculates the amount of 'time with night allowance'
    /// </summary>
    /// <param name="workedHours"></param>
    /// <returns>TimeSpan: Amount of time between 21.00 & 06.00</returns>
    public TimeSpan CalculateNightAllowance(ClockedHours workedHours)
    {
        if ((workedHours.ClockedIn.Hour >= 6 && workedHours.ClockedOut!.Value.Hour <= 21) ||
            workedHours.ClockedIn.DayOfWeek == DayOfWeek.Sunday || TodayIsHoliday(workedHours.ClockedIn.Date))
        {
            return TimeSpan.Zero;
        }

        var totalFeeTime = new TimeSpan(0);

        if (workedHours.ClockedIn.Hour < 6)
        {
            totalFeeTime += new TimeSpan(6, 0, 0) - workedHours.ClockedIn.TimeOfDay;
        }


        if (workedHours.ClockedOut!.Value.Hour >= 21 && workedHours.ClockedIn.DayOfWeek != DayOfWeek.Saturday)
        {
            var beginTimeOfFee = workedHours.ClockedIn.TimeOfDay;
            if (workedHours.ClockedIn.Hour < 21)
            {
                beginTimeOfFee = TimeSpan.FromHours(21);
            }

            totalFeeTime += workedHours.ClockedOut.Value.TimeOfDay - beginTimeOfFee;
        }

        return totalFeeTime;
    }

    /// <summary>
    /// Calculates the amount of 'time between 18 & 24 Saturday'
    /// </summary>
    /// <param name="workedHours"></param>
    /// <returns>TimeSpan: amount of time between 18.00 & 24.00 (Saturday only)</returns>
    public TimeSpan CalculateSaturday18Till24Allowance(ClockedHours workedHours)
    {
        if (workedHours.ClockedOut!.Value.Hour < 18 || workedHours.ClockedIn.DayOfWeek != DayOfWeek.Saturday ||
            TodayIsHoliday(workedHours.ClockedIn.Date))
        {
            return TimeSpan.Zero;
        }

        var beginTimeOfFee = new TimeSpan(18, 0, 0);
        if (workedHours.ClockedIn.TimeOfDay > beginTimeOfFee)
        {
            beginTimeOfFee = workedHours.ClockedIn.TimeOfDay;
        }

        return workedHours.ClockedOut.Value.TimeOfDay - beginTimeOfFee;
    }

    /// <summary>
    /// Calculates the amount of time worked on Sunday
    /// </summary>
    /// <param name="workedHours"></param>
    /// <returns>TimeSpan: amount of time on Sunday</returns>
    public TimeSpan CalculateSundayAllowance(ClockedHours workedHours)
    {
        if (workedHours.ClockedIn.DayOfWeek != DayOfWeek.Sunday)
        {
            return TimeSpan.Zero;
        }

        return workedHours.ClockedOut!.Value.TimeOfDay - workedHours.ClockedIn.TimeOfDay;
    }

    /// <summary>
    /// Calculates the amount of time worked on a official holiday (uses the 'DaysFinder' from Prognosis, credits to Mauro)
    /// </summary>
    /// <param name="workedHours"></param>
    /// <returns>TimeSpan: amount of Time on a Holiday</returns>
    public TimeSpan CalculateHolidayAllowance(ClockedHours workedHours)
    {
        if (!TodayIsHoliday(workedHours.ClockedIn.Date) || workedHours.ClockedIn.DayOfWeek == DayOfWeek.Sunday)
        {
            return TimeSpan.Zero;
        }

        return GetShiftDuration(workedHours.ClockedIn, workedHours.ClockedOut!.Value);
    }

    /// <summary>
    /// Decides whether the given date is a official Dutch holiday or not.
    /// </summary>
    /// <param name="date"></param>
    /// <returns>Boolean: True for Holiday - False for a default day</returns>
    public bool TodayIsHoliday(DateTime date)
    {
        var holidays = DaysFinder.GetAllHolidaysForThisYear();
        var holidayDates = new List<DateTime>();
        foreach (var holidayDate in holidays)
        {
            holidayDates.Add(holidayDate.GetDateForThisYear(date));
        }

        return holidayDates.Contains(date);
    }

    /// <summary>
    /// Calculates the duration of a shift, based on given start- & endtime
    /// </summary>
    /// <param name="startShift"></param>
    /// <param name="endShift"></param>
    /// <returns>TimeSpan: duration of shift</returns>
    public TimeSpan GetShiftDuration(DateTime startShift, DateTime endShift)
    {
        if (endShift <= startShift)
        {
            return TimeSpan.Zero;
        }

        return endShift - startShift;
    }

    /// <summary>
    /// Calculates the amount of 'default time (without fee) for a given workedHours (shift).
    /// </summary>
    /// <param name="workedHours"></param>
    /// <param name="timeWithFees"></param>
    /// <returns>TimeSpan: Amount of default time (without fee)</returns>
    public TimeSpan CalculateTimeDefault(ClockedHours workedHours, TimeSpan timeWithFees) // WorkedHours workedHours
    {
        if (timeWithFees > workedHours.ClockedOut!.Value.TimeOfDay - workedHours.ClockedIn.TimeOfDay)
        {
            return TimeSpan.Zero;
        }

        return workedHours.ClockedOut.Value.TimeOfDay - workedHours.ClockedIn.TimeOfDay - timeWithFees;
    }

    /// <summary>
    /// Calculates the amount of 'time between 20 & 21' for a non-weekend day.
    /// </summary>
    /// <param name="workedHours"></param>
    /// <returns>TimeSpan: Amount of time between 20.00 & 21.00</returns>
    public TimeSpan CalculateTimeBetween20And21Allowance(ClockedHours workedHours) // WorkedHours workedHours
    {
        var endOfFeeTime = new TimeSpan(21, 00, 00);
        if (workedHours.ClockedOut!.Value.Hour < 20 || workedHours.ClockedIn.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday || TodayIsHoliday(workedHours.ClockedIn.Date))
        {
            return TimeSpan.Zero;
        }

        if (endOfFeeTime > workedHours.ClockedOut.Value.TimeOfDay)
        {
            endOfFeeTime = workedHours.ClockedOut.Value.TimeOfDay;
        }

        return endOfFeeTime - new TimeSpan(20, 00, 00);
    }
}