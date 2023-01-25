using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bumbo.Data.DAL;

public class EmployeeAvailabilityRepository : IEmployeeAvailabilityRepository
{
    private readonly BumboDbContext _context;

    public EmployeeAvailabilityRepository(BumboDbContext context)
    {
        _context = context;
    }

    public async Task<IList<StandardAvailability>> GetStandardAvailabilities(string employeeId)
    {
        return await _context.StandardAvailabilities
            .Where(s => s.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task UpdateStandardAvailability(List<StandardAvailability> standardAvailabilities)
    {
        foreach (var standardAvailability in standardAvailabilities)
        {
            if (_context.StandardAvailabilities.Any(sa => sa.EmployeeId.Equals(standardAvailability.EmployeeId) &&
                                                          sa.DayOfWeek == standardAvailability.DayOfWeek))
            {
                if (standardAvailability.End - standardAvailability.Start == TimeSpan.Zero)
                {
                    standardAvailability.Start = new DateTime(2000, 1, 1, 0, 0, 0);
                    standardAvailability.End = new DateTime(2000, 1, 1, 0, 0, 0);
                }

                if (standardAvailability.Start.Equals(null) || standardAvailability.End.Equals(null)) continue;

                if (standardAvailability.Start.TimeOfDay.ToString().Equals(TimeOnly.MinValue.ToString()) ||
                    standardAvailability.End.TimeOfDay.ToString().Equals(TimeOnly.MinValue.ToString())) continue;

                _context.StandardAvailabilities.Update(standardAvailability);
            }
            else if ((standardAvailability.Start.Equals(null) || standardAvailability.End.Equals(null)))
            {
                standardAvailability.Start = new DateTime(2000, 1, 1, 0, 0, 0);
                standardAvailability.End = new DateTime(2000, 1, 1, 0, 0, 0);
                await _context.StandardAvailabilities.AddAsync(standardAvailability);
            }
            else
            {
                await _context.StandardAvailabilities.AddAsync(standardAvailability);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> RemoveStandardAvailability(DayOfWeek dayOfWeek, string employeeId)
    {
        var standardAvailability = await _context.StandardAvailabilities
            .FirstOrDefaultAsync(sa => sa.EmployeeId.Equals(employeeId) && sa.DayOfWeek == dayOfWeek);

        if (standardAvailability == null) return false;

        standardAvailability.Start = new DateTime(2000, 1, 1, 0, 0, 0);
        standardAvailability.End = new DateTime(2000, 1, 1, 0, 0, 0);

        _context.StandardAvailabilities.Update(standardAvailability);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task SetStandardSchoolHours(string employeeId, Dictionary<DayOfWeek, double> standardSchoolHoursPerDayOfWeek)
    {
        foreach (var dayOfWeekValue in Enum.GetNames(typeof(DayOfWeek)))
        {
            var dayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), dayOfWeekValue);
            var standardSchoolHours = new StandardSchoolHours()
            {
                EmployeeId = employeeId,
                DayOfWeek = dayOfWeek,
                Hours = (decimal)standardSchoolHoursPerDayOfWeek[dayOfWeek]
            };

            if (_context.StandardSchoolHours.Any(sh => sh.DayOfWeek.Equals(dayOfWeek) && sh.EmployeeId.Equals(employeeId)))
            {
                _context.StandardSchoolHours.Update(standardSchoolHours);
            }
            else
            {
                await _context.StandardSchoolHours.AddAsync(standardSchoolHours);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<double> GetStandardSchoolHoursForDayOfWeek(string employeeId, DayOfWeek dayOfWeek)
    {
        var x = await _context.StandardSchoolHours
            .Where(sh => sh.EmployeeId.Equals(employeeId) && sh.DayOfWeek.Equals(dayOfWeek))
            .Select(sh => sh.Hours)
            .FirstOrDefaultAsync();
        return (double)x;
    }

    public async Task<StandardAvailability> GetStandardAvailabilityForDayOfWeek(string employeeId, DayOfWeek dayOfWeek)
    {
        return (await _context.StandardAvailabilities
            .FirstOrDefaultAsync(sa => sa.EmployeeId.Equals(employeeId) && sa.DayOfWeek.Equals(dayOfWeek)))!;
    }

    public async Task<IList<SpecialAvailability>> GetSpecialAvailabilities(string employeeId)
    {
        return await _context.SpecialAvailabilities
            .Where(s => s.EmployeeId == employeeId
                        && s.Start.Date >= DateTime.Today)
            .OrderByDescending(x => x.Start.Date)
            .ToListAsync();
    }

    public async Task SetSpecialSchoolHours(string employeeId, SpecialSchoolHours specialSchoolHours)
    {
        var id = await _context.SpecialSchoolHours
            .Where(x => x.Start.Date == specialSchoolHours.Start.Date && x.EmployeeId == employeeId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        if (_context.SpecialSchoolHours.Any(sa => sa.EmployeeId.Equals(employeeId) && sa.Start.Date.Equals(specialSchoolHours.Start.Date)))
        {
            specialSchoolHours.Id = id;
            _context.SpecialSchoolHours.Update(specialSchoolHours);
        }
        else
        {
            await _context.SpecialSchoolHours.AddAsync(specialSchoolHours);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IList<SpecialSchoolHours>> GetSpecialSchoolHours(string employeeId)
    {
        return await _context.SpecialSchoolHours
            .Where(s => s.EmployeeId == employeeId
                        && s.Start.Date >= DateTime.Today)
            .OrderByDescending(s => s.Start.Date)
            .ToListAsync();
    }

    public async Task RemoveSpecialSchoolHour(int? id)
    {
        var exists = await _context.SpecialSchoolHours.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (exists != null)
        {
            _context.SpecialSchoolHours.Remove(exists);
        }

        await _context.SaveChangesAsync();
    }

    public async Task SetSpecialAvailability(string employeeId, SpecialAvailability specialAvailability)
    {
        var id = await _context.SpecialAvailabilities
            .Where(x => x.Start.Date == specialAvailability.Start.Date && x.EmployeeId == employeeId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        if (_context.SpecialAvailabilities.Any(sa => sa.EmployeeId.Equals(employeeId) && sa.Start.Date.Equals(specialAvailability.Start.Date) && sa.End.Date.Equals(specialAvailability.End.Date)))
        {
            specialAvailability.Id = id;
            _context.SpecialAvailabilities.Update(specialAvailability);
        }
        else
        {
            await _context.SpecialAvailabilities.AddAsync(specialAvailability);
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveSpecialAvailability(int? id)
    {
        var exists = await _context.SpecialAvailabilities.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (exists != null)
        {
            _context.SpecialAvailabilities.Remove(exists);
        }

        await _context.SaveChangesAsync();
    }
}