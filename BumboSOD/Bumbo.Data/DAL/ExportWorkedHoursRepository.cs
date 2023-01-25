using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bumbo.Data.DAL;

public class ExportWorkedHoursRepository : IExportWorkedHoursRepository
{
    private readonly BumboDbContext _context;

    public ExportWorkedHoursRepository(BumboDbContext context)
    {
        _context = context;
    }

    public async Task<Branch?> GetBranch(int? branchId)
    {
        return await _context.Branches
            .AsNoTracking()
            .Where(b => b.Id == branchId)
            .FirstOrDefaultAsync();
    }

    public async Task RenumerateClockedHours(int? branchId, DateTime startDate, DateTime endDate)
    {
        var clockedHours = await _context.ClockedHours
            .Where(ch => ch.Employee.BranchId == branchId && ch.ClockedIn.Date >= startDate && ch.ClockedOut.Value.Date <= endDate)
            .ToListAsync();

        clockedHours.ForEach(ch => ch.IsRemunerated = true);
        await _context.SaveChangesAsync();
    }
}