using Bumbo.Data.Models;

namespace Bumbo.WorkingRules.CAORules;

public interface IWorkingRules
{
    Task<Dictionary<Shift, List<string>>> CheckAllShiftsForRules(List<Shift> allShifts);

    Task<List<string>> CheckShiftForRules(Shift shift, ApplicationUser employee, List<Shift> allShiftsOfBranchFromPastMonth,
        List<StandardAvailability> allDefaultAvailabilitiesOfEmployees, List<SpecialAvailability> allSpecialAvailabilitiesOfEmployees,
        List<StandardSchoolHours> allDefaultSchoolHoursOfEmployees, List<SpecialSchoolHours> allSpecialSchoolHoursOfEmployees);

    int GetBreakMinutesForShift(Shift shift);

    public string CheckSchoolHoursFilledInMinor(DateTime time, ApplicationUser employee, List<SpecialSchoolHours> allSpecialSchoolHoursOfEmployees,
        List<StandardSchoolHours> allDefaultSchoolHoursOfEmployees);

    public string CheckMaxHoursPerDay(Shift shift, ApplicationUser employee, List<Shift> allShiftsOfEmployeeFromPastMonth,
        List<SpecialSchoolHours> allSpecialSchoolHoursOfEmployees, List<StandardSchoolHours> allDefaultSchoolHoursOfEmployees);

    public string CheckMaxDaysPerWeek(DateTime dateNewShift, ApplicationUser employee, List<Shift> allShifts);

    public IEnumerable<string> CheckMaxHoursPerWeek(Shift shift, ApplicationUser employee, List<Shift> allShiftsOfEmployeeFromPastMonth
        , List<SpecialSchoolHours> allSpecialSchoolHoursOfEmployees,
        List<StandardSchoolHours> allDefaultSchoolHoursOfEmployees);

    public IEnumerable<string> CheckEmployeeIsAvailableForShift(ApplicationUser employee, Shift shift, List<StandardAvailability> allDefaultAvailabilitiesOfEmployees,
        List<SpecialAvailability> allSpecialAvailabilitiesOfEmployees);

    string CheckMaxWorkTime(Shift shift, ApplicationUser employee);

    TimeSpan GetShiftDuration(DateTime startShift, DateTime endShift);
}