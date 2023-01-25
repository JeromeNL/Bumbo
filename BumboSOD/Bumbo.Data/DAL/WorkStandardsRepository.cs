using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Bumbo.Data.DAL;

public class WorkStandardsRepository : IWorkStandardsRepository
{
    private readonly BumboDbContext _context;
    private readonly IUserService _userService;

    public WorkStandardsRepository(BumboDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<WorkStandards?> GetUitladenWorkStandard()
    {
        var user = await _userService.GetUser();
        return await _context.WorkStandards.AsNoTracking()
            .Where(ws => ws.Task == WorkStandardTypes.Uitladen && ws.BranchId == user.BranchId)
            .OrderByDescending(ws => ws.DateEntered).FirstOrDefaultAsync();
    }

    public async Task<WorkStandards?> GetVakkenVullenWorkStandard()
    {
        var user = await _userService.GetUser();
        return await _context.WorkStandards.AsNoTracking()
            .Where(ws => ws.Task == WorkStandardTypes.VakkenVullen && ws.BranchId == user.BranchId)
            .OrderByDescending(ws => ws.DateEntered).FirstOrDefaultAsync();
    }

    public async Task<WorkStandards?> GetKassaWorkStandard()
    {
        var user = await _userService.GetUser();
        return await _context.WorkStandards.AsNoTracking()
            .Where(ws => ws.Task == WorkStandardTypes.Kassa && ws.BranchId == user.BranchId)
            .OrderByDescending(ws => ws.DateEntered).FirstOrDefaultAsync();
    }

    public async Task<WorkStandards?> GetVersWorkStandard()
    {
        var user = await _userService.GetUser();
        return await _context.WorkStandards.AsNoTracking()
            .Where(ws => ws.Task == WorkStandardTypes.Vers && ws.BranchId == user.BranchId)
            .OrderByDescending(ws => ws.DateEntered).FirstOrDefaultAsync();
    }

    public async Task<WorkStandards?> GetSpiegelenWorkStandard()
    {
        var user = await _userService.GetUser();
        return await _context.WorkStandards.AsNoTracking()
            .Where(ws => ws.Task == WorkStandardTypes.Spiegelen && ws.BranchId == user.BranchId)
            .OrderByDescending(ws => ws.DateEntered).FirstOrDefaultAsync();
    }

    public async Task AddWorkStandards(List<WorkStandards> workStandards)
    {
        foreach (var workStandard in workStandards)
        {
            _context.WorkStandards.Add(workStandard);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<WorkStandards>?> GetPastWorkStandardsList(WorkStandards? currentWorkStandard)
    {
        if (currentWorkStandard == null)
        {
            return null;
        }

        var user = await _userService.GetUser();
        // Retrieves the past work standards for the current branch in the form of a list
        return await _context.WorkStandards.AsNoTracking()
            .Where(ws => ws.BranchId == user.BranchId && ws.DateEntered != currentWorkStandard.DateEntered)
            .OrderByDescending(ws => ws.DateEntered).ToListAsync();
    }

    public async Task<Dictionary<DateTime, List<WorkStandards>>?> GetPastWorkStandardsDictionary(WorkStandards currentWorkStandard)
    {
        var pastWorkStandards = await GetPastWorkStandardsList(currentWorkStandard);

        if (pastWorkStandards == null || pastWorkStandards.Count == 0)
        {
            return null;
        }

        // Creates a dictionary with the date as key and a list of work standards as value
        var pastWorkStandardsDictionary = new Dictionary<DateTime, List<WorkStandards>>();

        // Loops through the list of past work standards
        foreach (var workStandard in pastWorkStandards)
        {
            // Checks if the dictionary already contains a key with the date of the current work standard, if not it adds it
            if (!pastWorkStandardsDictionary.ContainsKey(workStandard.DateEntered))
            {
                pastWorkStandardsDictionary.Add(workStandard.DateEntered, new List<WorkStandards>());
            }

            // Adds the current work standard to the list of work standards with the same date
            pastWorkStandardsDictionary[workStandard.DateEntered].Add(workStandard);
        }

        return pastWorkStandardsDictionary;
    }
}