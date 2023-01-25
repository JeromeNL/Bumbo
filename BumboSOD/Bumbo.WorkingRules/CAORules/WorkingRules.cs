using Bumbo.Data.Models;
using Bumbo.Data.Utils;
using Bumbo.WorkingRules.Repositories;

namespace Bumbo.WorkingRules.CAORules;

public class WorkingRules : IWorkingRules
{
    private const string _noconflicts = "No Conflicts";
    private readonly ICaoRepository _repo;

    public WorkingRules(ICaoRepository caoRepository)
    {
        _repo = caoRepository;
    }

    /// <summary>
    /// Checks the given shifts from a list for all CAO rules.
    /// </summary>
    /// <param name="allShifts"></param>
    /// <returns>A Dictionary of conflicts (strings) - Key: Shift Value: List of conflicts</returns>
    public async Task<Dictionary<Shift, List<string>>> CheckAllShiftsForRules(List<Shift> allShifts)
    {
        var allShiftsWithConflicts = new Dictionary<Shift, List<string>>();
        if (!allShifts.Any())
        {
            return allShiftsWithConflicts;
        }

        var weekAgo = allShifts.First().Start.Add(TimeSpan.FromDays(-8));
        var monthAgo = allShifts.First().Start.Add(TimeSpan.FromDays(-29));
        var firstDayOfWeek = allShifts.First().Start;
        var weekInFuture = allShifts.First().Start.Add(TimeSpan.FromDays(8));
        var currentBranchId = (int)allShifts.First().Employee.BranchId;


        // Get all DefaultAvailability for all employees of this branch for this week.
        var allDefaultAvailabilitiesOfEmployees = new List<StandardAvailability>();
        allDefaultAvailabilitiesOfEmployees.AddRange(await _repo.GetDefaultAvailabilityForPeriod(currentBranchId));

        // Get all SpecialAvailability of all employees of this branch for this week.
        var allSpecialAvailabilitiesOfEmployees = new List<SpecialAvailability>();
        allSpecialAvailabilitiesOfEmployees.AddRange(await _repo.GetSpecialAvailabilityForPeriod(allShifts.First().Start.Add(TimeSpan.FromDays(-40)),
            allShifts.First().Start.Add(TimeSpan.FromDays(15)), currentBranchId));

        // Get All DefaultSchoolhours for all employees of this branch for this week
        var allDefaultSchoolHoursOfEmployees = new List<StandardSchoolHours>();
        allDefaultSchoolHoursOfEmployees.AddRange(await _repo.GetDefaultSchoolHoursForPeriod(currentBranchId));

        // Get All SpecialSchoolHours for all employees of this branch for this week
        var allSpecialSchoolHoursOfEmployees = new List<SpecialSchoolHours>();
        allSpecialSchoolHoursOfEmployees.AddRange(await _repo.GetSpecialSchoolHoursForPeriod(weekAgo,
            weekInFuture, currentBranchId));

        // Get all shifts of all employees for this branch for the past 4 weeks
        var allShiftsOfEmployeeFromPastMonth = new List<Shift>();
        allShiftsOfEmployeeFromPastMonth.AddRange(await _repo.GetAllShiftsWithinPeriod(monthAgo,
            firstDayOfWeek, currentBranchId));


        foreach (var shift in allShifts)
        {
            var itemList = await CheckShiftForRules(shift, shift.Employee, allShiftsOfEmployeeFromPastMonth, allDefaultAvailabilitiesOfEmployees,
                allSpecialAvailabilitiesOfEmployees, allDefaultSchoolHoursOfEmployees, allSpecialSchoolHoursOfEmployees);

            if (itemList.Count > 0)
                allShiftsWithConflicts.Add(shift, itemList);
        }

        return allShiftsWithConflicts;
    }

    /// <summary>
    /// Checks the given shift for all CAO rules
    /// </summary>
    /// <param name="shift"></param>
    /// <param name="employee"></param>
    /// <returns>A list of conflicts for given shift</returns>
    public async Task<List<string>> CheckShiftForRules(Shift shift, ApplicationUser employee, List<Shift> allShiftsOfBranchFromPastMonth,
        List<StandardAvailability> allDefaultAvailabilitiesOfEmployees, List<SpecialAvailability> allSpecialAvailabilitiesOfEmployees,
        List<StandardSchoolHours> allDefaultSchoolHoursOfEmployees, List<SpecialSchoolHours> allSpecialSchoolHoursOfEmployees)
    {
        // Check basic values of the shift.
        CheckShiftIsValid(shift);

        // List of strings with conflicts. Is empty if there is no conflicts.
        var conflicts = new List<string>();

        // Add conflict if schoolhours are not filled in < 18y
        conflicts.Add(CheckSchoolHoursFilledInMinor(shift.Start, employee, allSpecialSchoolHoursOfEmployees, allDefaultSchoolHoursOfEmployees));

        // Add conflicts with 'Max hours per day'
        conflicts.Add(CheckMaxHoursPerDay(shift, employee, allShiftsOfBranchFromPastMonth, allSpecialSchoolHoursOfEmployees, allDefaultSchoolHoursOfEmployees));

        // Add conflics with 'Max hours per week'
        conflicts.AddRange(CheckMaxHoursPerWeek(shift, employee, allShiftsOfBranchFromPastMonth, allSpecialSchoolHoursOfEmployees, allDefaultSchoolHoursOfEmployees));

        // Add conflict with 'Max days per week < 16'
        conflicts.Add(CheckMaxDaysPerWeek(shift.Start, employee, allShiftsOfBranchFromPastMonth));

        // Add conflicts if employee <16 && shift > 19h
        conflicts.Add(CheckMaxWorkTime(shift, employee));

        // Add conflicts if employee is not available
        conflicts.AddRange(CheckEmployeeIsAvailableForShift(employee, shift, allDefaultAvailabilitiesOfEmployees, allSpecialAvailabilitiesOfEmployees));

        conflicts.RemoveAll(e => e == _noconflicts);
        return conflicts;
    }

    /// <summary>
    /// Get duration of a break for a given shift.
    /// </summary>
    /// <param name="shift"></param>
    /// <returns>Break duration in minutes</returns>
    public int GetBreakMinutesForShift(Shift shift)
    {
        var duration = shift.End - shift.Start;
        if (duration < TimeSpan.FromHours(4.5))
        {
            return 0;
        }

        if (duration >= TimeSpan.FromHours(4.5) && duration < TimeSpan.FromHours(8))
        {
            return 30;
        }

        if (duration >= TimeSpan.FromHours(8))
        {
            return 60;
        }

        return -1;
    }

    /// <summary>
    /// Checks whether the (special or standard) schoolhours are filled in for the week within the given date.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="employee"></param>
    /// <returns>A string of the conflict (or 'no conflicts')</returns>
    public string CheckSchoolHoursFilledInMinor(DateTime time, ApplicationUser employee, List<SpecialSchoolHours> allSpecialSchoolHoursOfEmployees,
        List<StandardSchoolHours> allDefaultSchoolHoursOfEmployees)
    {
        if (GetAgeOfEmployeeAtShift(employee.BirthDate, time) >= 18)
        {
            return _noconflicts;
        }

        if (!GetSchoolHoursFilledIn(employee, time, allSpecialSchoolHoursOfEmployees, allDefaultSchoolHoursOfEmployees))
        {
            return $"{employee.FirstName} heeft geen schooluren ingevuld.";
        }

        return _noconflicts;
    }

    /// <summary>
    /// Checks whether the employee is allowed to work the given shift, based on 'max hours per day'. Takes other shift(s) into account
    /// + schoolhours for minors.
    /// </summary>
    /// <param name="shift"></param>
    /// <param name="employee"></param>
    /// <returns>A string of the conflict (or 'no conflicts')</returns>
    public string CheckMaxHoursPerDay(Shift shift, ApplicationUser employee, List<Shift> allShiftsOfEmployeeFromPastMonth,
        List<SpecialSchoolHours> allSpecialSchoolHoursOfEmployees, List<StandardSchoolHours> allDefaultSchoolHoursOfEmployees)
    {
        var age = GetAgeOfEmployeeAtShift(employee.BirthDate, shift.Start);
        var plannedHours = GetPlannedHoursForDay(shift.Start, employee, allShiftsOfEmployeeFromPastMonth);
        var schoolHours = GetSchoolHoursDayForEmployee(employee, shift.Start, allSpecialSchoolHoursOfEmployees, allDefaultSchoolHoursOfEmployees);

        if (schoolHours == null)
        {
            schoolHours = TimeSpan.Zero;
        }

        if (age < 16)
        {
            if (plannedHours + schoolHours > TimeSpan.FromMinutes(8 * 60))
            {
                return ($"{employee.FirstName} is jonger dan 16, dus mag maar 8 uur werken per dag");
            }
        }

        if (age is >= 16 and < 18)
        {
            if (plannedHours + schoolHours > TimeSpan.FromMinutes(9 * 60))
            {
                return ($"{employee.FirstName} is jonger dan 18, dus mag maar 9 uur werken per dag");
            }
        }

        if (age >= 18)
        {
            if (plannedHours > TimeSpan.FromMinutes(12 * 60))
            {
                return ($"{employee.FirstName} mag maximaal 12 uur werken per dag");
            }
        }

        return _noconflicts;
    }

    /// <summary>
    /// Checks whether the employee is allowed to work the given shift, based on 'max workdays per week'. Only applicable for children under 16.
    /// </summary>
    /// <param name="dateNewShift"></param>
    /// <param name="employee"></param>
    /// <returns>A string of the conflict (or 'no conflicts')</returns>
    public string CheckMaxDaysPerWeek(DateTime dateNewShift, ApplicationUser employee, List<Shift> allShifts)
    {
        if (GetAgeOfEmployeeAtShift(employee.BirthDate, dateNewShift) < 16)
        {
            var monday = dateNewShift.StartOfWeek(DayOfWeek.Monday);
            var workedWeekDays = new Dictionary<DateTime, bool>();

            for (var i = 0; i < 7; i++)
            {
                workedWeekDays.Add(monday.AddDays(i).Date, false);
            }

            var allShiftsOfEmployee = GetShiftsForWeek(monday, employee, allShifts);
            foreach (var item in allShiftsOfEmployee)
            {
                workedWeekDays[item.Start.Date] = true;
            }

            var countWorkDays = 0;
            foreach (var item in workedWeekDays)
            {
                if (item.Value)
                {
                    countWorkDays++;
                }
            }

            if (countWorkDays > 5)
            {
                return $"{employee.FirstName} mag niet meer dan 5 dagen per week werken";
            }
        }

        return _noconflicts;
    }

    /// <summary>
    /// Checks whether the employee is allowed to work the given shift, based on 'max workinghours per week'.
    /// </summary>
    /// <param name="shift"></param>
    /// <param name="employee"></param>
    /// <returns>A list of conflicts (or 'no conflicts')</returns>
    public IEnumerable<string> CheckMaxHoursPerWeek(Shift shift, ApplicationUser employee, List<Shift> allShiftsOfEmployeeFromPastMonth
        , List<SpecialSchoolHours> allSpecialSchoolHoursOfEmployees,
        List<StandardSchoolHours> allDefaultSchoolHoursOfEmployees)
    {
        var conflicts = new List<string>();
        var age = GetAgeOfEmployeeAtShift(employee.BirthDate, shift.Start);
        var schoolHours = GetSchoolHoursForWeek(shift.Start, employee, allSpecialSchoolHoursOfEmployees, allDefaultSchoolHoursOfEmployees);
        var plannedHours = GetPlannedHoursForWeek(shift.Start, employee, allShiftsOfEmployeeFromPastMonth);

        if (age < 16)
        {
            if ((schoolHours + plannedHours) > TimeSpan.FromHours(40))
            {
                conflicts.Add($"{employee.FirstName} mag niet meer dan 40 uur per week werken");
            }

            if (schoolHours > TimeSpan.Zero && (plannedHours) > TimeSpan.FromHours(12))
            {
                conflicts.Add($"{employee.FirstName} mag niet meer dan 12 uur per schoolweek werken");
            }

            return conflicts;
        }

        if (age is >= 16 and < 18)
        {
            if (GetFourWeekAverageOfActiveHours(shift.Start, employee, allShiftsOfEmployeeFromPastMonth, allSpecialSchoolHoursOfEmployees, allDefaultSchoolHoursOfEmployees) > TimeSpan.FromHours(40))
            {
                conflicts.Add($"{employee.FirstName} mag gemiddeld niet meer dan 40 uur per week werken");
            }

            return conflicts;
        }

        if (age >= 18)
        {
            if (plannedHours > TimeSpan.FromHours(60))
            {
                conflicts.Add($"{employee.FirstName} mag niet meer dan 60 uur per week werken");
            }
        }

        return conflicts;
    }

    /// <summary>
    /// Checks till what time a employee is allowed to work. Under 16 years old == 19h, >=16h/o no restrictions.
    /// </summary>
    /// <param name="shift"></param>
    /// <param name="employee"></param>
    /// <returns>A string of the conflict (or 'no conflicts')</returns>
    public string CheckMaxWorkTime(Shift shift, ApplicationUser employee)
    {
        if (GetAgeOfEmployeeAtShift(employee.BirthDate, shift.Start) < 16 && shift.End.TimeOfDay > TimeSpan.FromHours(19))
        {
            return $"{employee.FirstName} mag niet na 19.00 werken.";
        }

        return _noconflicts;
    }

    /// <summary>
    /// Checks the availability of an employee. Firstly, checks the 'special availability' for the given day. If there is no special,
    /// the 'standard' will be checked.
    /// </summary>
    /// <param name="employee"></param>
    /// <param name="shift"></param>
    /// <returns>A list of conflicts (or 'no conflicts')</returns>
    public IEnumerable<string> CheckEmployeeIsAvailableForShift(ApplicationUser employee, Shift shift, List<StandardAvailability> allDefaultAvailabilitiesOfEmployees,
        List<SpecialAvailability> allSpecialAvailabilitiesOfEmployees)
    {
        var specialAvailability = allSpecialAvailabilitiesOfEmployees.FirstOrDefault(e => e.Start.Date == shift.Start.Date && e.EmployeeId == employee.Id);
        var standardAvailability = allDefaultAvailabilitiesOfEmployees.FirstOrDefault(e => e.DayOfWeek == shift.Start.DayOfWeek && e.EmployeeId == employee.Id);
        var conflicts = new List<string>();

        if (specialAvailability != null)
        {
            if (specialAvailability.End - specialAvailability.Start == TimeSpan.Zero)
            {
                conflicts.Add($"{employee.FirstName} is de gehele dag niet beschikbaar.");
                return conflicts;
            }

            if (shift.Start < specialAvailability.Start)
            {
                conflicts.Add($"{employee.FirstName} is pas beschikbaar vanaf {specialAvailability.Start.ToString("HH:mm")} ");
            }

            if (shift.End > specialAvailability.End)
            {
                conflicts.Add($"{employee.FirstName} is slechts beschikbaar tot {specialAvailability.End.ToString("HH:mm")} ");
            }

            if (conflicts.Count == 0)
            {
                conflicts.Add(_noconflicts);
            }

            return conflicts;
        }

        if (standardAvailability != null)
        {
            if (standardAvailability.End.TimeOfDay - standardAvailability.Start.TimeOfDay == TimeSpan.Zero)
            {
                conflicts.Add($"{employee.FirstName} is de gehele dag niet beschikbaar.");
                return conflicts;
            }

            if (shift.Start.TimeOfDay < standardAvailability.Start.TimeOfDay)
            {
                conflicts.Add($"{employee.FirstName} is pas beschikbaar vanaf {standardAvailability.Start.ToString("HH:mm")} ");
            }

            if (shift.End.TimeOfDay > standardAvailability.End.TimeOfDay)
            {
                conflicts.Add($"{employee.FirstName} is slechts beschikbaar tot {standardAvailability.End.ToString("HH:mm")} ");
            }

            if (conflicts.Count == 0)
            {
                conflicts.Add(_noconflicts);
            }

            return conflicts;
        }

        conflicts.Add($"{employee.FirstName} heeft geen beschikbaarheid opgegeven voor deze dag");
        return conflicts;
    }

    /// <summary>
    /// Get the duration of an shift.
    /// </summary>
    /// <param name="startShift"></param>
    /// <param name="endShift"></param>
    /// <returns>Timespan of given shift</returns>
    public TimeSpan GetShiftDuration(DateTime startShift, DateTime endShift)
    {
        if (endShift <= startShift)
        {
            return TimeSpan.Zero;
        }

        var duration = endShift - startShift;
        return duration;
    }

    /// <summary>
    /// Checks whether or not the Schoolhours are filled in.  Only applicable for minors.
    /// </summary>
    /// <param name="employee"></param>
    /// <param name="time"></param>
    /// <returns>Boolean true for 'filled in' false for 'not filled in' AND always true for 18+ y/o</returns>
    private bool GetSchoolHoursFilledIn(ApplicationUser employee, DateTime time, List<SpecialSchoolHours> allSpecialSchoolHoursOfEmployees,
        List<StandardSchoolHours> allDefaultSchoolHoursOfEmployees)
    {
        if (GetAgeOfEmployeeAtShift(employee.BirthDate, time) >= 18)
        {
            return true;
        }

        var monday = time.StartOfWeek(DayOfWeek.Monday);
        for (var i = 0; i < 5; i++)
        {
            var item = GetSchoolHoursDayForEmployee(employee, monday.AddDays(i).Date, allSpecialSchoolHoursOfEmployees, allDefaultSchoolHoursOfEmployees);
            if (item == null)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Check the age of a specific employee at the start of a shift. If an employee has its birthday between 'schedule date' and 'shift date',
    /// will the new age be used for the specific shift.
    /// </summary>
    /// <param name="birthday"></param>
    /// <param name="startShift"></param>
    /// <returns>Int age of employee at startdate of shift</returns>
    private int GetAgeOfEmployeeAtShift(DateTime birthday, DateTime startShift)
    {
        var age = startShift.Year - birthday.Year;
        if (birthday > startShift.AddYears(-age)) age--;

        return age;
    }

    /// <summary>
    /// Get the schoolhours for the given employee, for the given date.
    /// </summary>
    /// <param name="employee"></param>
    /// <param name="date"></param>
    /// <returns>Timespan of schoolhours for specific employee. Returns 'specialSchoolHours' if filled in, else 'defaultSchoolHours. Returns null if no
    /// schoolhours are provided.</returns>
    private TimeSpan? GetSchoolHoursDayForEmployee(ApplicationUser employee, DateTime date, List<SpecialSchoolHours> allSpecialSchoolHoursOfEmployees,
        List<StandardSchoolHours> allDefaultSchoolHoursOfEmployees)
    {
        var specialSchoolHoursDay = allSpecialSchoolHoursOfEmployees.FirstOrDefault((e => e.EmployeeId == employee.Id && e.Start.Date == date.Date));
        var specialSchoolHours = 0.0m;
        if (specialSchoolHoursDay != null)
        {
            specialSchoolHours = specialSchoolHoursDay.Hours;
        }

        var defaultSchoolHoursDay = allDefaultSchoolHoursOfEmployees.FirstOrDefault((e => e.EmployeeId == employee.Id && e.DayOfWeek == date.DayOfWeek));
        var defaultSchoolHours = 0.0m;
        if (defaultSchoolHoursDay != null)
        {
            defaultSchoolHours = defaultSchoolHoursDay.Hours;
        }

        if (specialSchoolHoursDay != null)
        {
            return TimeSpan.FromHours((double)specialSchoolHours);
        }

        if (defaultSchoolHoursDay != null)
        {
            return TimeSpan.FromHours((double)defaultSchoolHours);
        }

        return null;
    }

    /// <summary>
    /// Get the schoolhours for the given employee, for the given date.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="employee"></param>
    /// <returns>Total amount of schoolhours for specific week (TimeSpan) OR -1 when at least 1 day is not filled in.</returns>
    private TimeSpan GetSchoolHoursForWeek(DateTime time, ApplicationUser employee, List<SpecialSchoolHours> allSpecialSchoolHoursOfEmployees,
        List<StandardSchoolHours> allDefaultSchoolHoursOfEmployees)
    {
        var monday = time.StartOfWeek(DayOfWeek.Monday);

        var totalHoursWeek = TimeSpan.Zero;
        for (var i = 0; i < 7; i++)
        {
            var dayHours = GetSchoolHoursDayForEmployee(employee, monday.AddDays(i), allSpecialSchoolHoursOfEmployees, allDefaultSchoolHoursOfEmployees);

            if (dayHours == null)
            {
                return TimeSpan.Zero;
            }

            totalHoursWeek += dayHours.Value;
        }

        return totalHoursWeek;
    }

    /// <summary>
    /// Get amount of planned workhours for a specific employee for the week within the given date.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="employee"></param>
    /// <returns>Total amount of planned hours for specific week (TimeSpan)</returns>
    private TimeSpan GetPlannedHoursForWeek(DateTime time, ApplicationUser employee, List<Shift> allShiftsOfEmployeeFromPastMonth)
    {
        var monday = time.StartOfWeek(DayOfWeek.Monday);

        var totalHoursWeek = new TimeSpan(0);
        for (var i = 0; i < 7; i++)
        {
            totalHoursWeek += GetPlannedHoursForDay(monday.AddDays(i), employee, allShiftsOfEmployeeFromPastMonth);
        }

        return totalHoursWeek;
    }

    /// <summary>
    /// Get amount of planned workhours for a specific employee for the given day.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="employee"></param>
    /// <returns>Planned hours for specific day (TimeSpan)</returns>
    private TimeSpan GetPlannedHoursForDay(DateTime time, ApplicationUser employee, List<Shift> allShiftsOfEmployeeFromPastMonth)
    {
        var totalHoursPlannedDay = new TimeSpan(0);

        var dayShifts = allShiftsOfEmployeeFromPastMonth.Where(e => e.Start.Date == time.Date && e.EmployeeId == employee.Id);
        foreach (var item in dayShifts)
        {
            totalHoursPlannedDay += item.End - item.Start;
        }

        return totalHoursPlannedDay;
    }

    /// <summary>
    /// Get all shift for given employee, for the week within the given date.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="employee"></param>
    /// <returns>List of shifts for specific week</returns>
    private List<Shift> GetShiftsForWeek(DateTime time, ApplicationUser employee, List<Shift> allShifts)
    {
        var monday = time.StartOfWeek(DayOfWeek.Monday);

        var shiftsWeek = new List<Shift>();
        for (var i = 0; i < 7; i++)
        {
            var dayShifts2 = allShifts.Where(e => e.EmployeeId == employee.Id && e.Start.Date == monday.Date.AddDays(i));
            shiftsWeek.AddRange(dayShifts2);
        }

        return shiftsWeek;
    }

    /// <summary>
    /// Get '4 week average' of 'active' hours (work + school) for specific employee. For 16-17 y/o's only
    /// </summary>
    /// <param name="time"></param>
    /// <param name="employee"></param>
    /// <returns>4 week average' of work & schoolhours (TimeSpan)</returns>
    private TimeSpan GetFourWeekAverageOfActiveHours(DateTime time, ApplicationUser employee, List<Shift> allShiftsOfEmployeeFromPastMonth
        , List<SpecialSchoolHours> allSpecialSchoolHoursOfEmployees, List<StandardSchoolHours> allDefaultSchoolHoursOfEmployees)
    {
        var totalHours = new TimeSpan();
        for (var i = 0; i < 4; i++)
        {
            totalHours += GetSchoolHoursForWeek(time.AddDays(i * -7), employee, allSpecialSchoolHoursOfEmployees,
                              allDefaultSchoolHoursOfEmployees) +
                          GetPlannedHoursForWeek(time.AddDays(i * -7), employee, allShiftsOfEmployeeFromPastMonth);
        }

        return totalHours.Divide(4);
    }

    /// <summary>
    /// Checks whether a shift is valid: StartDate != EndDate && StartDate !> EndDate etc..
    /// </summary>
    /// <param name="shift"></param>
    /// <exception cref="ArgumentException"></exception>
    private void CheckShiftIsValid(Shift shift)
    {
        if (!shift.Start.DayOfWeek.Equals(shift.End.DayOfWeek))
        {
            throw new ArgumentException("End date should be on the same day as the start date");
        }

        if (shift.Start.Date > shift.End.Date || shift.End.Date < shift.Start.Date)
        {
            throw new ArgumentException("The end time should be after the start time");
        }
    }
}